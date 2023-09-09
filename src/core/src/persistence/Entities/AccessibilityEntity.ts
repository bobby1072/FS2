import { Entity, PrimaryColumn } from "typeorm";
import { AccessibilityDBType } from "../Schemas/AccessibilitySchema";
import BaseEntity from "./BaseEntity";

@Entity({ name: "accessibility" })
export default class AccessibilityEntity
  extends BaseEntity
  implements AccessibilityDBType
{
  @PrimaryColumn({ type: "text" })
  Title!: string;
  public async ToRuntimeTypeAsync() {}
  public ToRuntimeTypeSync() {}
}
