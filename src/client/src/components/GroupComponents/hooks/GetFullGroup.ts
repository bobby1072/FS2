import { useQuery } from "react-query";
import { GroupModel } from "../../../models/GroupModel";
import { ApiException } from "../../../common/ApiException";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";

export const useGetFullGroup = (groupId?: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<GroupModel, ApiException>(
    Constants.QueryKeys.GetFullGroup,
    () => {
      if (!groupId) throw new ApiException("No group id given");
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetFullGroup(groupId, user.access_token);
    }
  );
  return { ...queryResults };
};
