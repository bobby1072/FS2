import { Column, Entity, PrimaryColumn } from "typeorm";
import {
  WorldFishGenericDbSchema,
  WorldFishGenericDbSchemaType,
} from "../Schemas/WorldFishSchema";
import { BaseEntity } from "./BaseEntity";
import Fish from "../../common/RuntimeTypes/WorldFishGeneric";

@Entity({ name: "world_fish" })
export default class WorldFishGenericEntity
  extends BaseEntity
  implements WorldFishGenericDbSchemaType
{
  private static readonly _schema = WorldFishGenericDbSchema;
  @Column({ type: "text", nullable: true })
  A3Code?: string | null;
  @Column({ type: "text", nullable: true })
  EnglishName?: string | null;
  @Column({ type: "text", nullable: true })
  Isscaap?: number | null;
  @Column({ type: "text", nullable: true })
  Nickname?: string | null;
  @Column({ type: "text", nullable: true })
  ScientificName?: string | null;
  @PrimaryColumn({ type: "text" })
  Taxocode!: string;
  public async ToRuntimeTypeAsync(): Promise<Fish> {
    return new Fish(this.ToJson());
  }
  public ToRuntimeTypeSync(): Fish {
    return new Fish(this.ToJson());
  }
  public static ParseSync(val: any): WorldFishGenericEntity {
    const { Taxocode, A3Code, EnglishName, Isscaap, Nickname, ScientificName } =
      this._schema.parse(val);
    const tempObj = new WorldFishGenericEntity();
    tempObj.A3Code = A3Code;
    tempObj.EnglishName = EnglishName;
    tempObj.Isscaap = Isscaap;
    tempObj.Nickname = Nickname;
    tempObj.ScientificName = ScientificName;
    tempObj.Taxocode = Taxocode;
    return tempObj;
  }
  public static async ParseAsync(val: any) {
    return WorldFishGenericEntity.ParseSync(val);
  }
}
