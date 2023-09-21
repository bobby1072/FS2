import { BaseEntity } from "../../persistence/Entities/BaseEntity";
import UserRoleEntity from "../../persistence/Entities/UserRoleEntity";
import { DeepPartial } from "../DeepPartial";
import BaseRuntime from "./BaseRuntime";
import { UserRoleSchema, UserRoleType } from "./Schemas/UserRoleSchema";

export default class UserRole extends BaseRuntime implements UserRoleType {
  private static readonly _schema = UserRoleSchema;
  public RoleName: string;
  public GroupPermissions: string[];
  constructor({ RoleName, GroupPermissions = [] }: DeepPartial<UserRoleType>) {
    super();
    const { GroupPermissions: groupPermissions, RoleName: roleName } =
      UserRole._schema.parse({
        RoleName,
        GroupPermissions,
      });
    this.RoleName = roleName;
    this.GroupPermissions = groupPermissions;
    return this;
  }
  public ToEntity(): UserRoleEntity {
    return UserRoleEntity.ParseSync(this._toJson());
  }
  public ToEntityAsync(): Promise<UserRoleEntity> {
    return UserRoleEntity.ParseAsync(this._toJson());
  }
}
