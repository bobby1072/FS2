import { PermissionSchema, PermissionType } from "./Schemas/PermissionSchema";

export default class Permission implements PermissionType {
  public readonly Buzzword: string;
  public static readonly _schema = PermissionSchema;
  constructor({ buzzword }: { buzzword: string }) {
    const { Buzzword } = Permission._schema.parse({ Buzzword: buzzword });
    this.Buzzword = Buzzword;
  }
}
