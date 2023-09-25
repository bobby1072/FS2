import { Application, Request, Response } from "express";
import TokenData from "../common/RuntimeTypes/TokenData";
import User from "../common/RuntimeTypes/User";
import Constants from "../common/Constants";
import ApiError from "../common/ApiError";
import { ZodError } from "zod";
import BaseService from "../services/BaseService";
import BaseRepository from "../persistence/Repositories/BaseRepository";
import { BaseEntity } from "../persistence/Entities/BaseEntity";
import BaseRuntime from "../common/RuntimeTypes/BaseRuntime";
import UserService from "../services/UserService";

export default abstract class BaseController<
  TService extends BaseService<BaseRepository<BaseEntity, BaseRuntime>>
> {
  protected readonly _app: Application;
  protected readonly _service: TService;
  protected readonly _userService?: UserService;
  constructor(service: TService, app: Application, userService?: UserService) {
    this._service = service;
    this._app = app;
    this._userService = userService;
    return this;
  }
  protected _addAuthHandlingWithFullUser(
    routeFunc: (
      req: Request,
      resp: Response,
      userToken: TokenData,
      userFull: User
    ) => Promise<void>
  ) {
    return this._addAuthHandling(async (req, resp, token) => {
      const instanceCheck = this._service instanceof UserService;
      if (instanceCheck || this._userService) {
        const userServ = instanceCheck
          ? (this._service as any)
          : (this._userService as UserService);
        const foundUser = await userServ.LoginUserFromTokenWithoutPassword(
          token
        );
        await routeFunc(req, resp, token, foundUser);
      } else {
        throw new Error();
      }
    });
  }
  protected _addAuthHandling(
    routeFunc: (
      req: Request,
      resp: Response,
      userToken: TokenData
    ) => Promise<void>
  ) {
    return async (req: Request, resp: Response) => {
      if (!req.headers.authorization) {
        throw new ApiError(Constants.ExceptionMessages.invalidToken, 401);
      }
      const userToke = await TokenData.DecodeTokenAsync(
        req.headers.authorization
      );
      await routeFunc(req, resp, userToke);
    };
  }
  protected _addErrorHandling(
    routeFunc: (req: Request, resp: Response) => Promise<void>
  ): (req: Request, resp: Response) => Promise<void> {
    return async (req: Request, resp: Response) => {
      let status: number = 500;
      let message: string = Constants.ExceptionMessages.internalServerError;
      try {
        await routeFunc(req, resp);
      } catch (e) {
        if (e instanceof ApiError) {
          message = e.message;
          if (e.Status) {
            status = e.Status;
          }
        } else if (e instanceof ZodError) {
          status = 422;
          message = e.issues.reduce((acc, val) => {
            if (val.message === "Required") {
              return `${acc} Required values for ${val.path
                .map((x, index, array) => {
                  if (array.length > 1) {
                    return index === array.length - 1
                      ? `and '${x}'`
                      : `, '${x}'`;
                  } else {
                    return `'${x}'`;
                  }
                })
                .join("")}.`;
            } else {
              return `${acc} ${val.message}.`;
            }
          }, "");
        }
        resp.status(status).send(message);
      }
    };
  }
  protected abstract _applyDefaultMiddleWares?(
    routeFunc: (req: Request, resp: Response) => Promise<void>
  ): (req: Request, resp: Response) => Promise<void>;
  public abstract InvokeRoutes(): void;
}
