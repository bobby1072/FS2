import { useQuery } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import {
  IPartialGroupCatchModel,
  RuntimePartialGroupCatchModel,
} from "../../../models/IGroupCatchModel";
import { ApiException } from "../../../common/ApiException";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

export const useGetCatchesForUserQuery = (userId: string) => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const queryResults = useQuery<IPartialGroupCatchModel[], ApiException>(
    Constants.QueryKeys.GetCatchesForUser,
    () =>
      BackendApiServiceProvider.GetPartialCatchesForUser(
        userId,
        user.access_token
      )
  );
  return {
    ...queryResults,
    data: queryResults.data?.map((x) => new RuntimePartialGroupCatchModel(x)),
  };
};
