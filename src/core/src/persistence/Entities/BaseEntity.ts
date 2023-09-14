import BaseRuntime from "../../common/RuntimeTypes/BaseRuntime";

abstract class BaseEn {
  public abstract ToRuntimeTypeSync(): BaseRuntime;
  public abstract ToRuntimeTypeAsync(): Promise<BaseRuntime>;
}
export { BaseEn as BaseEntity };
