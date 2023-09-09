import { Repository } from "typeorm";
import BaseEntity from "../Entities/BaseEntity";

export default abstract class BaseRepository<TEntity extends BaseEntity> {
  protected readonly _repo: Repository<TEntity>;
  constructor(repo: Repository<TEntity>) {
    this._repo = repo;
    return this;
  }
}
