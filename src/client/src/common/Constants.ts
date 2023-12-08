enum QueryKeys {
  ClientConfig = "client-config",
  GetUser = "get-user",
}
enum ErrorMessages {}
export default abstract class Constants {
  public static readonly QueryKeys: typeof QueryKeys = QueryKeys;
  public static readonly ErrorMessages: typeof ErrorMessages = ErrorMessages;
}
