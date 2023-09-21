import UserRole from "../../common/RuntimeTypes/UserRole";
import UserRoleEntity from "../Entities/UserRoleEntity";
import BaseRepository from "./BaseRepository";

export default class UserRoleRepository extends BaseRepository<
  UserRoleEntity,
  UserRole
> {
  public async Get(userRole: UserRole | string) {
    return this._repo
      .createQueryBuilder()
      .where("role_name = :roleName", {
        roleName: typeof userRole === "string" ? userRole : userRole.RoleName,
      })
      .getOne()
      .then((x) => x?.ToRuntimeTypeAsync());
  }
}
