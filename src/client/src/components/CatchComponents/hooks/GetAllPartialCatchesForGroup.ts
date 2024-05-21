import { useQuery } from "react-query";
import {
  IPartialGroupCatchModel,
  RuntimePartialGroupCatchModel,
} from "../../../models/IGroupCatchModel";
import { ApiException } from "../../../common/ApiException";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";

export const useGetAllPartialCatchesForGroupQuery = (groupId: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<IPartialGroupCatchModel[], ApiException>(
    Constants.QueryKeys.GetAllPartialCatchesFroGroup,
    () => {
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetAllPartialCatchesForGroup(
        groupId,
        user.access_token
      );
    },
    {
      retry: (count) => count < 2,
    }
  );
  return {
    ...queryResults,
    data: queryResults.data?.map((x) => new RuntimePartialGroupCatchModel(x)),
  };
};
