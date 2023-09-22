import { Request, Response } from "express";
import BaseController from "./BaseController";
import User from "../common/RuntimeTypes/User";
import UserService from "../services/UserService";
import { UsernamePassword } from "./RequestBodySchema/UsernamePassword";
import Constants from "../common/Constants";

export default class UserController extends BaseController<UserService> {
  protected _applyDefaultMiddleWares(
    routeFunc: (req: Request, resp: Response) => Promise<void>
  ) {
    return this._addErrorHandling(async (req: Request, resp: Response) => {
      await routeFunc(req, resp);
    });
  }
  public LoginUser() {
    return this._app.post(
      "/user/login",
      this._applyDefaultMiddleWares(async (req, resp) => {
        const usernameAndPassword = UsernamePassword.parse(req.body);
        const dbUser = await this._service.LoginUser(usernameAndPassword);
        resp
          .status(200)
          .json({ token: User.EncodeToken(dbUser.Email, dbUser.RoleName) });
      })
    );
  }
  public RegisterUser() {
    return this._app.post(
      "/user/register",
      this._applyDefaultMiddleWares(async (req, resp) => {
        const reqBodyUser = new User(req.body);
        const dbUser = await this._service.RegisterUser(reqBodyUser);
        resp
          .status(200)
          .json({ token: User.EncodeToken(dbUser.Email, dbUser.RoleName) });
      })
    );
  }
  public UpdateUser() {
    return this._app.post(
      "/user/update",
      this._addErrorHandling(
        this._addAuthHandling(async (req, resp, userToken) => {
          const reqBodyUser = new User(req.body);
          const dbUser = await this._service.UpdateUser(
            reqBodyUser,
            userToken.user
          );
          resp
            .status(200)
            .json({ token: User.EncodeToken(dbUser.Email, dbUser.RoleName) });
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
