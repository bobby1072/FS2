import { useQuery } from "react-query";
import { ApiException } from "../../../common/ApiException";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import {
  IGroupCatchModel,
  RuntimeGroupCatchModel,
} from "../../../models/IGroupCatchModel";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import Constants from "../../../common/Constants";

export const useGetFullCatchQuery = (catchId?: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<IGroupCatchModel, ApiException>(
    Constants.QueryKeys.GetFullCatch,
    () => {
      if (!user) throw new Error("User is not authenticated");
      if (!catchId) throw new ApiException("No group id given");
      return BackendApiServiceProvider.GetFullCatchById(
        catchId,
        user.access_token
      );
    }
  );
  return {
    ...queryResults,
    data: queryResults?.data
      ? new RuntimeGroupCatchModel(queryResults.data)
      : undefined,
  };
};
