import { Fish } from "../../common/RuntimeTypes/WorldFishGeneric";
import WorldFishRepository from "../../persistence/Repositories/WorldFishRepository";
import BaseService from "../BaseService";
export default class WorldFishService extends BaseService<WorldFishRepository> {
  public async MigrateJsonFishDataToDb(): Promise<void> {
    const allFish: Fish[] = require("../../data/allFish.json");
    const allDbFish = await this._repo.GetAll();
    await this._repo.Create(allFish.filter((x) => !allDbFish.includes(x)));
  }
}
