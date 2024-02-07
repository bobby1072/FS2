import { useQuery } from "react-query";
import Constants from "../Constants";
import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { UserModel } from "../../models/UserModel";
import { useAuthentication } from "../contexts/AuthenticationContext";
import { ApiException } from "../ApiException";

export const useGetUserQuery = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery<UserModel, ApiException>(
    Constants.QueryKeys.GetUser,
    () => {
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetUser(user.access_token);
    }
  );
  return { ...queryResults };
};
