import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import { ApiException } from "../../../common/ApiException";

export const useDeleteGroupMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationResults = useMutation<
    string,
    ApiException,
    { groupId: string }
  >((g) => BackendApiServiceProvider.DeleteGroup(g.groupId, user.access_token));
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetAllListedGroups);
      queryClient.refetchQueries(Constants.QueryKeys.GetGroupCount);
      queryClient.refetchQueries(Constants.QueryKeys.GetSelfGroups);
      queryClient.refetchQueries(Constants.QueryKeys.GetGroupsWithChoice);
    }
  }, [mutationResults.data, queryClient]);
  return { ...mutationResults };
};
