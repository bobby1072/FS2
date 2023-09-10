import { BaseEntity } from "../../persistence/Entities/BaseEntity";
import PermissionEntity from "../../persistence/Entities/PermissionEntity";
import BaseRuntime from "./BaseRuntime";
import { PermissionSchema, PermissionType } from "./Schemas/PermissionSchema";

export default class Permission extends BaseRuntime implements PermissionType {
  public readonly Buzzword: string;
  private static readonly _schema = PermissionSchema;
  constructor({ buzzword }: { buzzword: string }) {
    super();
    const { Buzzword } = Permission._schema.parse({ Buzzword: buzzword });
    this.Buzzword = Buzzword;
    return this;
  }
  public ToEntity(): PermissionEntity {
    return PermissionEntity.ParseSync(this._toJson());
  }
  public async ToEntityAsync(): Promise<PermissionEntity> {
    return PermissionEntity.ParseAsync(this._toJson());
  }
}
