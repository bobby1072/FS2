import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";
import { SaveGroupPositionInput } from "../GroupPositionModal";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import Constants from "../../../common/Constants";

export const useSavePositionMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationResults = useMutation<
    string,
    ApiException,
    SaveGroupPositionInput
  >((gp) => BackendApiServiceProvider.SaveGroupPosition(gp, user.access_token));
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetGroupAndPositions);
    }
  }, [mutationResults.data, queryClient]);
  return { ...mutationResults };
};
