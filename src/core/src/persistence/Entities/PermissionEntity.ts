import { Entity, PrimaryColumn } from "typeorm";
import BaseEntity from "./BaseEntity";
import { PermissionDBType } from "../Schemas/PermissionSchema";

@Entity({ name: "permission" })
export default class PermissionEntity
  extends BaseEntity
  implements PermissionDBType
{
  @PrimaryColumn({ type: "text" })
  Buzzword!: string;
  public ToRuntimeTypeSync() {}
  public async ToRuntimeTypeAsync() {}
}
