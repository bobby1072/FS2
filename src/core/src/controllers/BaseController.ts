import { Application, Request, Response, application } from "express";
import TokenData from "../common/RuntimeTypes/TokenData";
import User from "../common/RuntimeTypes/User";
import Constants from "../common/Constants";
import ApiError from "../common/ApiError";
import { ZodError } from "zod";
import BaseService from "../services/BaseService";
import BaseRepository from "../persistence/Repositories/BaseRepository";
import { BaseEntity } from "../persistence/Entities/BaseEntity";
import BaseRuntime from "../common/RuntimeTypes/BaseRuntime";

export default abstract class BaseController<
  TService extends BaseService<BaseRepository<BaseEntity, BaseRuntime>>
> {
  protected readonly _app: Application;
  protected readonly _service: TService;
  constructor(service: TService, app: Application) {
    this._service = service;
    this._app = app;
    return this;
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
        throw new ApiError(Constants.ExceptionMessages.invalidToken);
      }
      const userToke = User.DecodeToken(req.headers.authorization);
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
        if (e instanceof Error && e.message) {
          message = e.message;
        }
        if (e instanceof ApiError && e.Status) {
          status = e.Status;
        }
        if (e instanceof ZodError) {
          status = 401;
          message = e.issues.reduce((acc, val) => {
            if (val.message === "Required") {
              return `${acc} Required values for ${val.path
                .map((x, index, array) => {
                  if (array.length > 1) {
                    return index === array.length - 1 ? `and ${x}` : `, ${x}`;
                  } else {
                    return x;
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
