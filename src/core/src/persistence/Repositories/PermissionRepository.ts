import Permission from "../../common/RuntimeTypes/Permission";
import PermissionEntity from "../Entities/PermissionEntity";
import BaseRepository from "./BaseRepository";

export default class PermissionRepository extends BaseRepository<PermissionEntity> {
  public async GetAll(): Promise<Permission[]> {
    return this._repo
      .createQueryBuilder()
      .getMany()
      .then((x) => Promise.all(x.map((y) => y.ToRuntimeTypeAsync())));
  }
}
