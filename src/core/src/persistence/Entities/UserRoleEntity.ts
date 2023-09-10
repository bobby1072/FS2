import { Column, Entity, PrimaryColumn } from "typeorm";
import { UserRoleDBType } from "../Schemas/UserRoleSchema";
import { BaseEntity } from "./BaseEntity";

@Entity({ name: "user_role" })
export default class UserRoleEntity
  extends BaseEntity
  implements UserRoleDBType
{
  @PrimaryColumn({ type: "text" })
  RoleName!: string;

  @Column({ type: "string", array: true, default: [] })
  GroupPermissions!: string[];

  @Column({ type: "string", array: true, default: [] })
  UserPermissions!: string[];

  public async ToRuntimeTypeAsync() {}
  public ToRuntimeTypeSync() {}
}
