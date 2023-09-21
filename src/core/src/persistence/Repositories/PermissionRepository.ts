import Permission from "../../common/RuntimeTypes/Permission";
import PermissionEntity from "../Entities/PermissionEntity";
import BaseRepository from "./BaseRepository";

export default class PermissionRepository extends BaseRepository<
  PermissionEntity,
  Permission
> {
  public async Get(
    permission: Permission | string
  ): Promise<Permission | undefined> {
    return this._repo
      .createQueryBuilder()
      .where("buzzword = :buzzword", {
        buzzword:
          typeof permission === "string" ? permission : permission.Buzzword,
      })
      .getOne()
      .then((x) => x?.ToRuntimeTypeAsync());
  }
}
