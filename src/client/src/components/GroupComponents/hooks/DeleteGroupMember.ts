import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import Constants from "../../../common/Constants";

export const useDeleteGroupMember = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  if (!user) throw new Error("User not found");
  const mutationResults = useMutation<
    string,
    ApiException,
    { groupMemberId: string }
  >((gm) =>
    BackendApiServiceProvider.DeleteGroupMember(
      gm.groupMemberId,
      user.access_token
    )
  );
  useEffect(() => {
    if (mutationResults.data)
      queryClient.refetchQueries(Constants.QueryKeys.GetAllMembersForGroup);
  }, [queryClient, mutationResults.data]);
  return { ...mutationResults };
};
