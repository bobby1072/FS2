enum QueryKeys {
  ClientConfig = "client-config",
}
enum ErrorMessages {}
export default abstract class Constants {
  public static readonly QueryKeys: typeof QueryKeys = QueryKeys;
}
