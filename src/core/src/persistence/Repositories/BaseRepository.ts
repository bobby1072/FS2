import { Repository } from "typeorm";
import { BaseEntity } from "../Entities/BaseEntity";
import BaseRuntime from "../../common/RuntimeTypes/BaseRuntime";
import ApiError from "../../common/ApiError";
import Constants from "../../common/Constants";

export default abstract class BaseRepository<
  TEntity extends BaseEntity,
  TRuntime extends BaseRuntime
> {
  protected readonly _repo: Repository<TEntity>;
  constructor(repo: Repository<TEntity>) {
    this._repo = repo;
    return this;
  }
  public async GetAll(): Promise<TRuntime[]> {
    return this._repo
      .createQueryBuilder()
      .getMany()
      .then((x) => Promise.all(x.map((y) => y.ToRuntimeTypeAsync() as any)));
  }
  public async UpdatePrimaryKeyOfRecord(
    oldPrimaryKey: any,
    newVal: BaseRuntime,
    entityType: typeof BaseEntity,
    primaryKeyName: string
  ) {
    return this._repo
      .createQueryBuilder()
      .update(entityType)
      .set(await newVal.ToEntityAsync())
      .where(`${primaryKeyName} = :oldKey`, { oldKey: oldPrimaryKey })
      .execute()
      .then((data) => true)
      .catch((error) => {
        throw new ApiError(Constants.ExceptionMessages.failedToUpdateUser, 500);
      });
  }
}
