import { Request, Response } from "express";
import WorldFishService from "../services/WorldFishService/WorldFishService";
import BaseController from "./BaseController";
import TokenData from "../common/RuntimeTypes/TokenData";
import ApiError from "../common/ApiError";
import Constants from "../common/Constants";
import { Fish } from "../common/RuntimeTypes/WorldFishGeneric";

export default class WorldFishController extends BaseController<WorldFishService> {
  protected _applyDefaultMiddleWares(
    routeFunc: (
      req: Request,
      resp: Response,
      userToke: TokenData
    ) => Promise<void>
  ) {
    return this._addErrorHandling(
      this._addAuthHandling(async (req, resp, userToke) => {
        await routeFunc(req, resp, userToke);
      })
    );
  }
  public GetAllLocalFish() {
    return this._app.get(
      "/worldfish/all",
      this._applyDefaultMiddleWares(async (req, resp, user) => {
        resp
          .status(200)
          .json((await this._service.GetAllFish()).map((x) => x.ToJson()));
      })
    );
  }
  public GetAllDetailsForFish() {
    return this._app.post(
      "/worldfish/fish",
      this._applyDefaultMiddleWares(async (req, resp, user) => {
        const userFish = new Fish(req.body);
        const extendedFish = await this._service.GetFullFish(userFish);
        resp.status(200).json(extendedFish.ToJson());
      })
    );
  }
  public GetSimilarFish() {
    return this._app.get(
      "/worldfish/similar/",
      this._applyDefaultMiddleWares(async (req, resp, userToke) => {
        const searchTerm = req.query.searchTerm;
        if (
          !(typeof searchTerm === "string" && /^[A-Za-z\s]*$/.test(searchTerm))
        ) {
          throw new ApiError(
            Constants.ExceptionMessages.expectedStringForSearchTerm,
            422
          );
        }
        resp
          .status(200)
          .json(
            (await this._service.SearchForSimilarLocalFish(searchTerm)).map(
              (x) => x.ToJson()
            )
          );
      })
    );
  }
  public InvokeRoutes(): void {
    this.GetAllDetailsForFish();
    this.GetAllLocalFish();
    this.GetSimilarFish();
  }
}
