import { Entity, PrimaryColumn } from "typeorm";
import BaseEntity from "./BaseEntity";
import { PermissionDBType } from "../Schemas/PermissionSchema";
import Permission from "../../common/RuntimeTypes/Permission";

@Entity({ name: "permission" })
export default class PermissionEntity
  extends BaseEntity
  implements PermissionDBType
{
  @PrimaryColumn({ type: "text" })
  Buzzword!: string;
  public ToRuntimeTypeSync(): Permission {
    return new Permission({ buzzword: this.Buzzword });
  }
  public async ToRuntimeTypeAsync(): Promise<Permission> {
    return new Permission({ buzzword: this.Buzzword });
  }
}
