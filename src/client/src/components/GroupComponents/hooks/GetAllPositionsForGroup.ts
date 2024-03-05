import { useQuery } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { GroupPositionModel } from "../../../models/GroupPositionModel";
import { ApiException } from "../../../common/ApiException";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

export const useGetAllPositionsForGroup = (groupId?: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<GroupPositionModel[], ApiException>(
    Constants.QueryKeys.GetAllPositionsForGroup,
    () => {
      if (!groupId) throw new ApiException("No group id given");
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetAllPositionsForGroup(
        groupId,
        user?.access_token
      );
    },
    {
      retry: (fc, e) => (e.status === 404 ? false : fc < 3),
    }
  );
  return { ...queryResults };
};
