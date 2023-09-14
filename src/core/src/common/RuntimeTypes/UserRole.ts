import { BaseEntity } from "../../persistence/Entities/BaseEntity";
import UserRoleEntity from "../../persistence/Entities/UserRoleEntity";
import BaseRuntime from "./BaseRuntime";
import { UserRoleSchema, UserRoleType } from "./Schemas/UserRoleSchema";

export default class UserRole extends BaseRuntime implements UserRoleType {
  private static readonly _schema = UserRoleSchema;
  public RoleName: string;
  public GroupPermissions: string[];
  constructor({
    roleName,
    groupPermissions = [],
  }: {
    roleName: string;
    groupPermissions?: string[];
  }) {
    super();
    const { GroupPermissions, RoleName } = UserRole._schema.parse({
      RoleName: roleName,
      GroupPermissions: groupPermissions,
    });
    this.RoleName = RoleName;
    this.GroupPermissions = GroupPermissions;
    return this;
  }
  public ToEntity(): UserRoleEntity {
    return UserRoleEntity.ParseSync(this._toJson());
  }
  public ToEntityAsync(): Promise<UserRoleEntity> {
    return UserRoleEntity.ParseAsync(this._toJson());
  }
}
