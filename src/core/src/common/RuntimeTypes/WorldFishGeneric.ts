import WorldFishGenericEntity from "../../persistence/Entities/WorldFishGenericEntity";
import BaseRuntime from "./BaseRuntime";
import {
  WorldFishGenericSchema,
  WorldFishGenericSchemaType,
} from "./Schemas/WorldFishSchema";

class WorldFishGeneric
  extends BaseRuntime
  implements WorldFishGenericSchemaType
{
  private readonly _schema = WorldFishGenericSchema;
  public readonly ScientificName?: string | null;
  public readonly A3Code?: string | null;
  public readonly EnglishName?: string | null;
  public readonly Isscaap?: number | null;
  public readonly Taxocode: string;
  public readonly Nickname?: string | null;
  constructor(input: WorldFishGenericSchemaType) {
    super();
    const { A3Code, EnglishName, Isscaap, ScientificName, Taxocode, Nickname } =
      this._schema.parse(input);
    this.A3Code = A3Code;
    this.ScientificName = ScientificName;
    this.EnglishName = EnglishName;
    this.Isscaap = Isscaap;
    this.Taxocode = Taxocode;
    this.Nickname = Nickname;
    return this;
  }
  public ToEntity(): WorldFishGenericEntity {
    return WorldFishGenericEntity.ParseSync(this.ToJson());
  }
  public async ToEntityAsync(): Promise<WorldFishGenericEntity> {
    return WorldFishGenericEntity.ParseAsync(this.ToJson());
  }
}
export { WorldFishGeneric as Fish };
