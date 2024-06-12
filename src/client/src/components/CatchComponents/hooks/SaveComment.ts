import { useMutation, useQueryClient } from "react-query";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { useEffect } from "react";
import Constants from "../../../common/Constants";
import { SaveCommentInput } from "../CatchCommentForm";

export const useSaveCommentMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  const mutationResults = useMutation<number, ApiException, SaveCommentInput>(
    (a) => {
      if (!user) throw new Error("User is not authenticated");
      return BackendApiServiceProvider.SaveComment(a, user.access_token);
    }
  );
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetCatchComments);
    }
  }, [mutationResults.data, queryClient]);
  return { ...mutationResults };
};
