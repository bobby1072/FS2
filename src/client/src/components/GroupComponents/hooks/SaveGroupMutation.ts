import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import Constants from "../../../common/Constants";
import { useEffect } from "react";
import { ApiException } from "../../../common/ApiException";

export const useSaveGroupMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationProps = useMutation<string, ApiException, FormData>((g) =>
    BackendApiServiceProvider.SaveGroup(user.access_token, g)
  );
  useEffect(() => {
    if (mutationProps.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetAllListedGroups);
      queryClient.refetchQueries(Constants.QueryKeys.GetGroupCount);
      queryClient.refetchQueries(Constants.QueryKeys.GetSelfGroups);
      queryClient.refetchQueries(Constants.QueryKeys.GetGroupsWithChoice);
      queryClient.refetchQueries(Constants.QueryKeys.GetUser);
    }
  }, [mutationProps.data, queryClient]);
  return {
    ...mutationProps,
  };
};
