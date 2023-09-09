import { Application, Request, Response } from "express";
import { UserApiRequestBodySchema } from "./RequestSchemas/UserRequestBodySchema";
import BaseController from "./BaseController";
import User from "../common/RuntimeTypes/User";
import UserService from "../services/UserService";
import UserRepository from "../persistence/Repositories/UserRepository";
import UserEntity from "../persistence/Entities/UserEntity";

export default class UserController extends BaseController<
  UserEntity,
  UserRepository,
  UserService
> {
  private _addPostValidation(
    routeFunc: (req: Request, resp: Response, user: User) => Promise<void>
  ) {
    return async (req: Request, resp: Response) => {
      const { email, password, phoneNumber } = UserApiRequestBodySchema.parse(
        req.body
      );
      await routeFunc(
        req,
        resp,
        new User({ email, pass: password, phoneNum: phoneNumber })
      );
    };
  }
  protected _applyMiddleWare(
    routeFunc: (req: Request, resp: Response, user: User) => Promise<void>
  ) {
    return this._addErrorHandling(async (req: Request, resp: Response) => {
      await this._addPostValidation(routeFunc)(req, resp);
    });
  }
  public LoginUser() {
    return this._app.post(
      "/user/login",
      this._applyMiddleWare(async (req, resp, user) => {
        const dbUser = await this._service.LoginUser(user);
        resp.status(200).json({ token: User.EncodeToken(dbUser.Email) });
      })
    );
  }
  public RegisterUser() {
    return this._app.post(
      "/user/register",
      this._applyMiddleWare(async (req, resp, user) => {
        const dbUser = await this._service.RegisterUser(user);
        resp.status(200).json({ token: User.EncodeToken(dbUser.Email) });
      })
    );
  }
  public UpdateUser() {
    return this._app.post(
      "/user/update",
      this._addErrorHandling(
        this._addAuthHandling(async (req, resp, userToken) => {
          const { email, password, phoneNumber } =
            UserApiRequestBodySchema.parse(req.body);
          const newUser = new User({
            email,
            pass: password,
            phoneNum: phoneNumber,
          });
          const dbUser = await this._service.UpdateUser(
            newUser,
            userToken.user
          );
          resp.status(200).json({ token: User.EncodeToken(dbUser.Email) });
        })
      )
    );
  }
  public DeleteUser() {
    return this._app.get(
      "/user/delete",
      this._addErrorHandling(
        this._addAuthHandling(async (req, resp, userToken) => {
          await this._service.DeleteUser(userToken.user);
          resp.status(200).send("Succesfully deleted user");
        })
      )
    );
  }
  public InvokeRoutes(): void {
    this.LoginUser();
    this.RegisterUser();
    this.UpdateUser();
    this.DeleteUser();
  }
}
