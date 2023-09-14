import { BaseEntity } from "../persistence/Entities/BaseEntity";
import BaseRepository from "../persistence/Repositories/BaseRepository";

export default abstract class BaseService<
  TRepo extends BaseRepository<BaseEntity> | BaseRepository<BaseEntity>
> {
  protected readonly _repo: TRepo;
  constructor(repo: TRepo) {
    this._repo = repo;
    return this;
  }
}
