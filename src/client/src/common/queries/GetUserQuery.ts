import { useQuery } from "react-query";
import Constants from "../Constants";
import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { UserModel } from "../../models/UserModel";
import { useAuthentication } from "../contexts/AuthenticationContext";

export const useGetUserQuery = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery<UserModel, Error>(
    Constants.QueryKeys.GetUser,
    () => {
      if (!user?.id_token) throw new Error("No bearer token found");
      return BackendApiServiceProvider.GetUser(user.id_token);
    }
  );
  return { ...queryResults };
};
