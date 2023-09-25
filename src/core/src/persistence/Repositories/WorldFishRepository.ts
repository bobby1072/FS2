import { Fish } from "../../common/RuntimeTypes/WorldFishGeneric";
import WorldFishGenericEntity from "../Entities/WorldFishGenericEntity";
import BaseRepository from "./BaseRepository";

export default class WorldFishRepository extends BaseRepository<
  WorldFishGenericEntity,
  Fish
> {
  public async GetMany(
    anyName: string,
    exact: boolean = false
  ): Promise<Fish[]> {
    return this._repo
      .createQueryBuilder("w")
      .where(`w.english_name ${exact ? "=" : "LIKE"} :name`, { anyName })
      .orWhere(`w.scientific_name ${exact ? "=" : "LIKE"} :name`, { anyName })
      .orWhere(`w.nickname ${exact ? "=" : "LIKE"} :name`, { anyName })
      .getMany()
      .then((x) => Promise.all(x.map((y) => y.ToRuntimeTypeAsync())));
  }
  public async GetOne(
    name: string,
    name_type: "english_name" | "scientific_name" | "nickname"
  ): Promise<Fish | undefined> {
    return this._repo
      .createQueryBuilder("w")
      .where(`${name_type} = :name`, { name })
      .getOne()
      .then((x) => x?.ToRuntimeTypeAsync());
  }
  public async Create(fish: Fish[]): Promise<Fish[]> {
    const dbUSers = await this._repo.save(
      await Promise.all(fish.map((x) => x.ToEntityAsync()))
    );
    return Promise.all(dbUSers.map((x) => x.ToRuntimeTypeAsync()));
  }
}
