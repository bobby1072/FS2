enum QueryKeys {
  ClientConfig = "client-config",
  GetUser = "get-user",
  GetUserConstantRefresh = "get-user-constant-refresh",
  GetGroupCount = "get-group-count",
  GetSelfGroups = "get-self-groups",
  GetGroupsWithChoice = "get-groups-with-choice",
  DeleteGroup = "delete-group",
  GetAllListedGroups = "get-all-listed-groups",
  GetFullGroup = "get-full-group",
}
enum ErrorMessages {}
export default abstract class Constants {
  public static readonly QueryKeys: typeof QueryKeys = QueryKeys;
  public static readonly ErrorMessages: typeof ErrorMessages = ErrorMessages;
}
