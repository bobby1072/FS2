import { useQuery } from "react-query";
import { IUserWithoutEmailModel } from "../../../models/IUserModel";
import { ApiException } from "../../../common/ApiException";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";

export const useGetUserQuery = (userId: string) => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const queryResults = useQuery<
    IUserWithoutEmailModel & { email?: string | null },
    ApiException
  >(Constants.QueryKeys.GetUser, () =>
    BackendApiServiceProvider.GetUser(userId, user.access_token)
  );
  return { ...queryResults };
};
