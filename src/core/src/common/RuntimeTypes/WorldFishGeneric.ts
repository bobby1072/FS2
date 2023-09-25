import WorldFishGenericEntity from "../../persistence/Entities/WorldFishGenericEntity";
import WorldFishApiServiceProvider from "../../services/WorldFishService/WorldFishApiServiceProvider";
import BaseRuntime from "./BaseRuntime";
import {
  WorldFishGenericSchema,
  WorldFishGenericSchemaType,
} from "./Schemas/WorldFishSchema";

export class Fish extends BaseRuntime implements WorldFishGenericSchemaType {
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
    const speciesNumbers = await WorldFishApiServiceProvider.GetSpeciesNumbers(
      this.A3Code ? this.A3Code : ""
    );
    return new FishExtended({
      ...this,
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
export class FishExtended extends Fish {
  public readonly PhysicalDescription?: string | null;
  public readonly SpeciesPhoto?: string | null;
  public readonly SpeciesNumbers?: object[] | null;
  constructor(
    fishExtended: WorldFishGenericSchemaType & {
      PhysicalDescription?: string | null;
      SpeciesPhoto?: string | null;
      SpeciesNumbers?: object[] | null;
    }
  ) {
    super(fishExtended);
    this.PhysicalDescription = fishExtended.PhysicalDescription;
    this.SpeciesPhoto = fishExtended.SpeciesPhoto;
    this.SpeciesNumbers = fishExtended.SpeciesNumbers;
    return this;
  }
}
