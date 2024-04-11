import { useMutation, useQuery } from "react-query";
import { IGroupModel } from "../../../models/IGroupModel";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";

export const useGetAllListedGroups = (startIndex: number, count: number) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<
    Omit<IGroupModel, "positions" | "members" | "leader">[],
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
    Omit<IGroupModel, "positions" | "members" | "leader">[],
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

export const useSearchAllListedGroupsMutation = (options?: {
  retry: (count: number, exception: ApiException) => boolean;
}) => {
  const { user } = useAuthentication();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationResults = useMutation<
    IGroupModel[],
    ApiException,
    { groupName: string }
  >(
    (gn) =>
      BackendApiServiceProvider.SearchListedGroups(
        gn.groupName,
        user.access_token
      ),
    {
      ...(options?.retry && { retry: options.retry }),
    }
  );
  return { ...mutationResults };
};
