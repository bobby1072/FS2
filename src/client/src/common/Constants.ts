enum QueryKeys {
  ClientConfig = "client-config",
  GetUser = "get-user",
  GetGroupCount = "get-group-count",
  GetAllListedGroups = "get-all-listed-groups",
}
enum ErrorMessages {}
export default abstract class Constants {
  public static readonly QueryKeys: typeof QueryKeys = QueryKeys;
  public static readonly ErrorMessages: typeof ErrorMessages = ErrorMessages;
}
