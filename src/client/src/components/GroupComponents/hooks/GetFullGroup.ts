import { useQuery } from "react-query";
import { GroupModel } from "../../../models/GroupModel";
import { ApiException } from "../../../common/ApiException";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { GroupMemberModel } from "../../../models/GroupMemberModel";

export const useGetFullGroup = (groupId?: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<Omit<GroupModel, "members">, ApiException>(
    Constants.QueryKeys.GetGroupAndPositions,
    () => {
      if (!groupId) throw new ApiException("No group id given");
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetGroupAndPositions(
        groupId,
        user.access_token
      );
    },
    {
      retry: false,
    }
  );
  return { ...queryResults };
};

export const useGetAllMembers = (groupId?: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<GroupMemberModel[], ApiException>(
    Constants.QueryKeys.GetAllMembersForGroup,
    () => {
      if (!groupId) throw new ApiException("No group id given");
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetGroupMembers(
        groupId,
        user.access_token
      );
    },
    {
      retry: false,
    }
  );
  return { ...queryResults };
};
