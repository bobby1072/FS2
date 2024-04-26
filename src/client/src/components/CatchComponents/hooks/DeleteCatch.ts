import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { ApiException } from "../../../common/ApiException";
import { useEffect } from "react";
import Constants from "../../../common/Constants";

export const useDeleteCatchMutation = () => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const queryClient = useQueryClient();
  const mutationResults = useMutation<string, ApiException, string>((catchId) =>
    BackendApiServiceProvider.DeleteGroupCatch(catchId, user.access_token)
  );
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(
        Constants.QueryKeys.GetAllPartialCatchesFroGroup
      );
    }
  }, [queryClient, mutationResults.data]);
  return { ...mutationResults };
};
