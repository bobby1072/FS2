import { BaseEntity } from "../../persistence/Entities/BaseEntity";

export default abstract class BaseRuntime {
  public abstract ToEntity(): BaseEntity;
  public abstract ToEntityAsync(): Promise<BaseEntity>;
  public ToJson(): any {
    const parsObj = { ...this } as any;
    if ("_schema" in parsObj) {
      parsObj._schema = undefined;
    }
    return JSON.parse(JSON.stringify(parsObj));
  }
}
