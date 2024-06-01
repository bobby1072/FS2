import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import Constants from "../../../common/Constants";

export const useSaveCatchMutation = () => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const queryClient = useQueryClient();
  const mutationResults = useMutation<string, ApiException, FormData>((gc) =>
    BackendApiServiceProvider.SaveGroupCatch(gc, user.access_token)
  );
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(
        Constants.QueryKeys.GetAllPartialCatchesFroGroup
      );
      queryClient.refetchQueries(Constants.QueryKeys.GetFullCatch);
    }
  }, [queryClient, mutationResults.data]);
  return { ...mutationResults };
};
