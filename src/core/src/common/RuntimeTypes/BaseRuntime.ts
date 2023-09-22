import { BaseEntity } from "../../persistence/Entities/BaseEntity";

export default abstract class BaseRuntime {
  public abstract ToEntity(): BaseEntity;
  public abstract ToEntityAsync(): Promise<BaseEntity>;
  public ToJson(): any {
    return JSON.parse(JSON.stringify(this));
  }
}
