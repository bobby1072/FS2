import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import Constants from "../../../common/Constants";

export const useDeleteCommentMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  const mutationResults = useMutation<number, ApiException, number>((a) => {
    if (!user) throw new Error("User is not authenticated");
    return BackendApiServiceProvider.DeleteComment(a, user.access_token)
    }
  );
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetCatchComments);
    }
  }, [mutationResults.data, queryClient]);
  return { ...mutationResults };
};
