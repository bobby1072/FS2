import ApiError from "../../common/ApiError";
import Constants from "../../common/Constants";
import { Fish, FishExtended } from "../../common/RuntimeTypes/WorldFishGeneric";
import WorldFishRepository from "../../persistence/Repositories/WorldFishRepository";
import BaseService from "../BaseService";
export default class WorldFishService extends BaseService<WorldFishRepository> {
  public async MigrateJsonFishDataToDb(): Promise<void> {
    const allFish: Fish[] = require("../../data/allFish.json").map(
      (x: any) =>
        new Fish({
          Taxocode: x.taxocode,
          A3Code: x.a3_code,
          EnglishName: x.english_name,
          Isscaap: Number(x.isscaap) ? x.isscaap : undefined,
          ScientificName: x.scientific_name,
        })
    );
    const allDbFish = await this._repo.GetAll();
    const savedFish = await this._repo.Create(
      allFish.filter((x) => !allDbFish.includes(x))
    );
  }
  public async SearchForSimilarLocalFish(searchTerm: string): Promise<Fish[]> {
    const alikeFish = await this._repo.GetMany(searchTerm);
    if (alikeFish.length <= 0) {
      throw new ApiError(Constants.ExceptionMessages.noFishFound, 404);
    }
    return alikeFish;
  }
  public async GetFullFish(fishInput: Fish): Promise<FishExtended> {
    return fishInput.GetExtendedFish();
  }
  public async GetAllFish(): Promise<Fish[]> {
    return this._repo.GetAll();
  }
}
