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

  @Column({ type: "string", array: true, default: [] })
  GroupPermissions!: string[];

  @Column({ type: "string", array: true, default: [] })
  UserPermissions!: string[];

  public async ToRuntimeTypeAsync() {
    return new UserRole({
      roleName: this.RoleName,
      userPermissions: this.UserPermissions,
      groupPermissions: this.GroupPermissions,
    });
  }
  public ToRuntimeTypeSync() {
    return new UserRole({
      roleName: this.RoleName,
      userPermissions: this.UserPermissions,
      groupPermissions: this.GroupPermissions,
    });
  }
  public static ParseSync(val: any): UserRoleEntity {
    const { GroupPermissions, RoleName, UserPermissions } =
      this._schema.parse(val);
    const tempObj = new UserRoleEntity();
    tempObj.GroupPermissions = GroupPermissions;
    tempObj.UserPermissions = UserPermissions;
    tempObj.RoleName = RoleName;
    return tempObj;
  }
  public static async ParseAsync(val: any): Promise<UserRoleEntity> {
    return UserRoleEntity.ParseSync(val);
  }
  public static TryParseSync(val: any): UserRoleEntity | undefined {
    try {
      return UserRoleEntity.ParseSync(val);
    } catch (e) {
      return undefined;
    }
  }
  public static async TryParseAsync(
    val: any
  ): Promise<UserRoleEntity | undefined> {
    return this.TryParseSync(val);
  }
}
