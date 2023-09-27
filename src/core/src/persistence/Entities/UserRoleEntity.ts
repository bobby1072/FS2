import { Column, Entity, PrimaryColumn } from "typeorm";
import { UserRoleDBType, UserRoleDbSchema } from "../Schemas/UserRoleSchema";
import { BaseEntity } from "./BaseEntity";
import UserRole from "../../common/RuntimeTypes/UserRole";

@Entity({ name: "user_role" })
export default class UserRoleEntity
  extends BaseEntity
  implements UserRoleDBType
{
  private static readonly _schema = UserRoleDbSchema;
  @PrimaryColumn({ type: "text" })
  RoleName!: string;

  @Column({ type: "text", array: true, default: [] })
  GroupPermissions!: string[];

  public async ToRuntimeTypeAsync() {
    return new UserRole({
      ...this,
    });
  }
  public ToRuntimeTypeSync() {
    return new UserRole({
      ...this,
    });
  }
  public static ParseSync(val: any): UserRoleEntity {
    const { GroupPermissions, RoleName } = this._schema.parse(val);
    const tempObj = new UserRoleEntity();
    tempObj.GroupPermissions = GroupPermissions;
    tempObj.RoleName = RoleName;
    return tempObj;
  }
}
