import { BaseEntity } from "../../persistence/Entities/BaseEntity";
import PermissionEntity from "../../persistence/Entities/PermissionEntity";
import { DeepPartial } from "../DeepPartial";
import BaseRuntime from "./BaseRuntime";
import { PermissionSchema, PermissionType } from "./Schemas/PermissionSchema";

export default class Permission extends BaseRuntime implements PermissionType {
  public readonly Buzzword: string;
  private static readonly _schema = PermissionSchema;
  constructor({ Buzzword }: DeepPartial<Permission>) {
    super();
    const { Buzzword: buzzword } = Permission._schema.parse({
      Buzzword,
    });
    this.Buzzword = buzzword;
    return this;
  }
  public ToEntity(): PermissionEntity {
    return PermissionEntity.ParseSync(this.ToJson());
  }
  public async ToEntityAsync(): Promise<PermissionEntity> {
    return PermissionEntity.ParseSync(this.ToJson());
  }
}
