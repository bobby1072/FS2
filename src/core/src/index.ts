import compression from "compression";
import express, { Application } from "express";
import bodyParser from "body-parser";
import BaseController from "./controllers/BaseController";
import { DataSource } from "typeorm";
import UserEntity from "./persistence/Entities/UserEntity";
import UserRepository from "./persistence/Repositories/UserRepository";
import { serve, setup } from "swagger-ui-express";
import UserService from "./services/UserService";
import CronJobService from "./services/CronJobService/CronJobService";
import { ICronJobService } from "./services/CronJobService/ICronJobService";
import BaseRepository from "./persistence/Repositories/BaseRepository";
import BaseService from "./services/BaseService";
import { SnakeNamingStrategy } from "typeorm-naming-strategies";
import { BaseEntity } from "./persistence/Entities/BaseEntity";
import UserRoleEntity from "./persistence/Entities/UserRoleEntity";
import PermissionEntity from "./persistence/Entities/PermissionEntity";
import UserRoleRepository from "./persistence/Repositories/UserRoleRepository";
import PermissionRepository from "./persistence/Repositories/PermissionRepository";
import BaseRuntime from "./common/RuntimeTypes/BaseRuntime";
import UserController from "./controllers/UserController";
const SwaggerDoc = require("./swagger.json");
abstract class Program {
  private static readonly _portVar: number = Number(process.env.PORT) || 5000;
  private static readonly _app: Application = express();
  private static readonly _dbClient: DataSource = new DataSource({
    type: "postgres",
    database: "fs",
    synchronize: true,
    schema: "public",
    username: process.env.MAINDBUSER ?? "postgres",
    password: process.env.MAINDBPASSWORD ?? "postgres",
    ssl: false,
    port: Number(process.env.POSTGRESPORT) || 5560,
    host: process.env.POSTGRESHOST ?? "localhost",
    entities: [UserEntity, UserRoleEntity, PermissionEntity],
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

    await this._dbClient.initialize().then((x) => {
      console.log(
        `\nDB connection initialised\n\n${x.options.database}\n${x.options.type}\n`
      );
    });

    const [userRepo, userRoleRepo, permissionRepo] = [
      new UserRepository(this._dbClient.getRepository(UserEntity)),
      new UserRoleRepository(this._dbClient.getRepository(UserRoleEntity)),
      new PermissionRepository(this._dbClient.getRepository(PermissionEntity)),
    ];

    const [userService] = [new UserService(userRepo, userRoleRepo)];

    const controllers: BaseController<
      BaseService<BaseRepository<BaseEntity, BaseRuntime>>
    >[] = [new UserController(userService, Program._app)];

    const jobService: ICronJobService = new CronJobService(
      userService,
      this._dbClient
    );

    await jobService.RegisterAllJobs().then(() => {
      console.log(`\nAll Jobs successfully registered\n`);
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
