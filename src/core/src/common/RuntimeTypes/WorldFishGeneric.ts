import WorldFishGenericEntity from "../../persistence/Entities/WorldFishGenericEntity";
import WorldFishApiServiceProvider from "../../services/WorldFishService/WorldFishApiServiceProvider";
import BaseRuntime from "./BaseRuntime";
import {
  WorldFishGenericSchema,
  WorldFishGenericSchemaType,
} from "./Schemas/WorldFishSchema";
import FishExtended from "./WorldFishExtended";

export default class Fish
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
    this.Nickname = Nickname;
    if (EnglishName) {
      const { alias, englishName } = Fish._getNickNameAndName(EnglishName);
      if (alias && !Nickname) {
        this.Nickname = alias;
      }
      this.EnglishName = englishName;
    } else {
      this.EnglishName = EnglishName;
    }
    this.Isscaap = Isscaap;
    this.Taxocode = Taxocode;
    return this;
  }
  public ToEntity(): WorldFishGenericEntity {
    return WorldFishGenericEntity.ParseSync(this.ToJson());
  }
  public async ToEntityAsync(): Promise<WorldFishGenericEntity> {
    return WorldFishGenericEntity.ParseAsync(this.ToJson());
  }
  public async GetExtendedFish() {
    const [speciesInfo, speciesNumbers] = await Promise.all([
      WorldFishApiServiceProvider.GetFishInfo(
        this.EnglishName ? this.EnglishName : ""
      ),
      WorldFishApiServiceProvider.GetSpeciesNumbers(
        this.A3Code ? this.A3Code : ""
      ),
    ]);
    return new FishExtended({
      ...this,
      SpeciesPhoto: speciesInfo?.SpeciesPhoto,
      PhysicalDescription: speciesInfo?.PhysicalDescription,
      SpeciesNumbers: speciesNumbers,
    });
  }
  private static _getNickNameAndName(engName: string) {
    const fishNameAka = /\(([^)]+)\)/.exec(engName);
    let aka = fishNameAka && fishNameAka[1].replace(/[=]/g, " ");
    if (aka && aka.charAt(0) === " ") {
      aka = aka.substring(1);
    }
    const fishNameFixed: string = fishNameAka
      ? engName.replace(fishNameAka[0], " ")
      : engName;
    return { englishName: fishNameFixed, alias: aka };
  }
}
