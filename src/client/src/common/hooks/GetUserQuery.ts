import { useQuery } from "react-query";
import Constants from "../Constants";
import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { IUserModel, IUserWithPermissionsRawModel } from "../../models/IUserModel";
import { useAuthentication } from "../contexts/AuthenticationContext";
import { ApiException } from "../ApiException";

export const useGetUserQuery = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery<IUserModel, ApiException>(
    Constants.QueryKeys.GetUser,
    () => {
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetUser(user.access_token);
    }
  );
  return { ...queryResults };
};


export const useGetUserWithPermissionsQuery = () => {
  const { user } = useAuthentication();
  if (!user?.access_token) throw new ApiException("No bearer token found");
  const queryResults = useQuery<IUserWithPermissionsRawModel, ApiException>(
    Constants.QueryKeys.GetUser,
    () => {
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetUserWithGroupPermissions(user.access_token);
    }
  );
  return { ...queryResults };
};