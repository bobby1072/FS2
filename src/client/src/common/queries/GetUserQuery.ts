import { useQuery } from "react-query";
import Constants from "../Constants";
import { useAuthentication } from "../login/Authentication";
import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { UserModel } from "../../models/UserModel";

export const useGetUserQuery = () => {
  const { bearerToken } = useAuthentication();

  const queryResults = useQuery<UserModel, Error>(
    Constants.QueryKeys.GetUser,
    () => {
      if (!bearerToken) throw new Error("No bearer token found");
      return BackendApiServiceProvider.GetUser(bearerToken);
    }
  );
  return { ...queryResults };
};
