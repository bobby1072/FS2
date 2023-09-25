import BaseRuntime from "../../common/RuntimeTypes/BaseRuntime";

abstract class BaseEn {
  public abstract ToRuntimeTypeSync(): BaseRuntime;
  public abstract ToRuntimeTypeAsync(): Promise<BaseRuntime>;
  public ToJson(): any {
    return JSON.parse(JSON.stringify(this));
  }
}
export { BaseEn as BaseEntity };
