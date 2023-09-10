import { BaseEntity } from "typeorm";

abstract class BaseEn extends BaseEntity {
  public abstract ToRuntimeTypeSync(): any;
  public abstract ToRuntimeTypeAsync(): Promise<any>;
}
export { BaseEn as BaseEntity };
