import compression from "compression";
import express, { Application } from "express";
import bodyParser from "body-parser";
import BaseController from "./controllers/BaseController";
import { DataSource } from "typeorm";
import UserEntity from "./persistence/Entities/UserEntity";
import UserController from "./controllers/UserController";
import UserRepository from "./persistence/Repositories/UserRepository";
import { serve, setup } from "swagger-ui-express";
import UserService from "./services/UserService";
import CronJobService from "./services/CronJobService/CronJobService";
import { ICronJobService } from "./services/CronJobService/ICronJobService";
import ApiError from "./common/ApiError";
import Constants from "./common/Constants";
import BaseEntity from "./persistence/Entities/BaseEntity";
import BaseRepository from "./persistence/Repositories/BaseRepository";
import BaseService from "./services/BaseService";
import { SnakeNamingStrategy } from "typeorm-naming-strategies";
const SwaggerDoc = require("./swagger.json");
abstract class Program {
  private static readonly _portVar: number = Number(process.env.PORT) || 5000;
  private static readonly _app: Application = express();
  private static readonly _pgClient: DataSource = new DataSource({
    type: "postgres",
    database: "fs",
    synchronize: true,
    schema: "public",
    username: process.env.MAINDBUSER ?? "postgres",
    password: process.env.MAINDBPASSWORD ?? "postgres",
    ssl: false,
    port: Number(process.env.POSTGRESPORT) || 5560,
    host: process.env.POSTGRESHOST ?? "localhost",
    entities: [UserEntity],
    namingStrategy: new SnakeNamingStrategy(),
  });
  public static async Main(): Promise<void> {
    Program._app.use(compression());
    Program._app.use(bodyParser.urlencoded({ extended: true }));
    Program._app.use(bodyParser.json());
    if (process.env.NODE_ENV === "development") {
      Program._app.use("/api-docs", serve, setup(SwaggerDoc));
      console.log(`\nSwagger available at /api-docs\n`);
    }

    await this._pgClient.initialize().then((x) => {
      console.log(
        `\nDB connection initialised\n${x.options.database}\n${x.options.type}\n`
      );
    });

    const [userRepo] = [
      new UserRepository(this._pgClient.getRepository(UserEntity)),
    ];

    const [userService] = [new UserService(userRepo)];

    const controllers: BaseController<
      BaseEntity,
      BaseRepository<BaseEntity>,
      BaseService<BaseRepository<BaseEntity>>
    >[] = [new UserController(userService, this._app)];

    const jobService: ICronJobService = new CronJobService(
      userService,
      this._pgClient
    );

    await jobService.RegisterAllJobs().then((jobs) => {
      if (jobs === true) {
        console.log(`\nAll Jobs successfully registered\n`);
      } else {
        throw new ApiError(
          Constants.ExceptionMessages.failedToRegisterJobs,
          500
        );
      }
    });

    await Promise.all(
      controllers.map((x) => (async () => x.InvokeRoutes())())
    ).then(() => {
      console.log("\nControllers invoked\n");
    });

    this._app.listen(this._portVar, "0.0.0.0", () => {
      console.log(`\n\nServer running on port: ${this._portVar}\n\n`);
    });
  }
}
Program.Main();
