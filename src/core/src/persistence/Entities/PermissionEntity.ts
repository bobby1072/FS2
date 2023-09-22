import { Entity, PrimaryColumn } from "typeorm";
import {
  PermissionDBSchema,
  PermissionDBType,
} from "../Schemas/PermissionSchema";
import Permission from "../../common/RuntimeTypes/Permission";
import { BaseEntity } from "./BaseEntity";

@Entity({ name: "permission" })
export default class PermissionEntity
  extends BaseEntity
  implements PermissionDBType
{
  private static readonly _schema = PermissionDBSchema;
  @PrimaryColumn({ type: "text" })
  Buzzword!: string;

  public ToRuntimeTypeSync(): Permission {
    return new Permission(this.ToJson());
  }

  public async ToRuntimeTypeAsync(): Promise<Permission> {
    return new Permission(this.ToJson());
  }

  public static ParseSync(val: any): PermissionEntity {
    const { Buzzword } = this._schema.parse(val);
    const tempObj = new PermissionEntity();
    tempObj.Buzzword = Buzzword;
    return tempObj;
  }
  public static async ParseAsync(val: any): Promise<PermissionEntity> {
    return PermissionEntity.ParseSync(val);
  }
}
