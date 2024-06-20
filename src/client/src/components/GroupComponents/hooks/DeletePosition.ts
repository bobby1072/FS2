import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import Constants from "../../../common/Constants";

export const useDeletePositionMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  if (!user) throw new Error("User not found");
  const mutationResults = useMutation<
    number,
    ApiException,
    {
      positionId: number;
    }
  >((p) =>
    BackendApiServiceProvider.DeletePosition(p.positionId, user.access_token)
  );
  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetGroupAndPositions);
      queryClient.refetchQueries(Constants.QueryKeys.GetAllMembersForGroup);
    }
  }, [mutationResults.data, queryClient]);
  return { ...mutationResults };
};
