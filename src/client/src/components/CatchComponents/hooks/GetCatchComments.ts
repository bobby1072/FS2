import { useQuery } from "react-query";
import Constants from "../../../common/Constants";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { IGroupCatchCommentModel } from "../../../models/IGroupCatchComment";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { ApiException } from "../../../common/ApiException";

export const useGetCatchCommentsQuery = (catchId: string) => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const queryResults = useQuery<IGroupCatchCommentModel[], ApiException>(
    Constants.QueryKeys.GetCatchComments,
    () => {
      return BackendApiServiceProvider.GetCatchComments(
        catchId,
        user.access_token
      );
    }
  );
  return { ...queryResults };
};
