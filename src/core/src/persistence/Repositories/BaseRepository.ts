import { Repository } from "typeorm";
import { BaseEntity } from "../Entities/BaseEntity";
import BaseRuntime from "../../common/RuntimeTypes/BaseRuntime";

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
}
