import { useMutation, useQueryClient } from "react-query";
import { UserModel } from "../../../models/UserModel";
import { ApiException } from "../../../common/ApiException";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useEffect } from "react";
import Constants from "../../../common/Constants";
import { useCurrentUser } from "../../../common/UserContext";

export const useSaveUsernameMutation = () => {
  const { user } = useAuthentication();
  const {} = useCurrentUser();
  if (!user) {
    throw new Error("User not found");
  }
  const queryClient = useQueryClient();
  const mutationResult = useMutation<
    UserModel,
    ApiException,
    { newUsername: string }
  >((x) =>
    BackendApiServiceProvider.SaveNewUsername(user.access_token, x.newUsername)
  );
  useEffect(() => {
    if (mutationResult.data) {
      queryClient.refetchQueries(Constants.QueryKeys.GetUser);
    }
  }, [mutationResult.data, queryClient]);
  return { ...mutationResult };
};
