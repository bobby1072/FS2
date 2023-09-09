export default abstract class BaseEntity {
  public abstract ToRuntimeTypeSync(): any;
  public abstract ToRuntimeTypeAsync(): Promise<any>;
}
