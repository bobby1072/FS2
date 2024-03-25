enum QueryKeys {
  ClientConfig = "client-config",
  GetUser = "get-user",
  GetUserConstantRefresh = "get-user-constant-refresh",
  GetGroupCount = "get-group-count",
  GetSelfGroups = "get-self-groups",
  GetGroupsWithChoice = "get-groups-with-choice",
  DeleteGroup = "delete-group",
  GetAllListedGroups = "get-all-listed-groups",
  GetGroupAndPositions = "get-group-and-positions",
  GetAllPositionsForGroup = "get-all-positions-for-group",
  GetAllMembersForGroup = "get-all-members-for-group",
}
export default abstract class Constants {
  public static readonly QueryKeys: typeof QueryKeys = QueryKeys;
}
