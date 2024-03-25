import { useQuery } from "react-query";
import { GroupModel } from "../../../models/GroupModel";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";

export const useGetAllListedGroups = (startIndex: number, count: number) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<
    Omit<GroupModel, "positions" | "members" | "leader">[],
    ApiException
  >(Constants.QueryKeys.GetAllListedGroups, () => {
    if (!user?.access_token) throw new ApiException("No bearer token found");
    return BackendApiServiceProvider.GetAllListedGroups(
      user.access_token,
      startIndex,
      count
    );
  });
  return { ...queryResults };
};

export enum GroupQueryChoice {
  AllListed,
  SelfLead,
}
export const useGetAllGroupsChoiceGroup = (
  startIndex: number,
  count: number,
  choice: GroupQueryChoice,
  options?: { retry: (count: number, exception: ApiException) => boolean }
) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<
    Omit<GroupModel, "positions" | "members" | "leader">[],
    ApiException
  >(
    Constants.QueryKeys.GetGroupsWithChoice,
    () => {
      if (!user?.access_token) throw new ApiException("No bearer token found");
      switch (choice) {
        case GroupQueryChoice.AllListed:
          return BackendApiServiceProvider.GetAllListedGroups(
            user.access_token,
            startIndex,
            count
          );
        case GroupQueryChoice.SelfLead:
          return BackendApiServiceProvider.GetSelfGroups(
            user.access_token,
            startIndex,
            count
          );
        default:
          throw new ApiException("Invalid query");
      }
    },
    {
      ...(options?.retry && { retry: options.retry }),
    }
  );
  return { ...queryResults };
};
