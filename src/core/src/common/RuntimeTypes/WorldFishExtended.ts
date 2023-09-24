import { WorldFishGenericSchemaType } from "./Schemas/WorldFishSchema";
import { Fish } from "./WorldFishGeneric";

class WorldFishExtended extends Fish {
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
    return this;
  }
}
export { WorldFishExtended as FishExtended };
