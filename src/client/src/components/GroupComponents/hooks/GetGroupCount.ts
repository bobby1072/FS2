import { useQuery } from "react-query";
import Constants from "../../../common/Constants";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { ApiException } from "../../../common/ApiException";

export const useGetGroupCount = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery<number, ApiException>(
    Constants.QueryKeys.GetGroupCount,
    () => {
      if (!user?.access_token) throw new ApiException("No bearer token found");
      return BackendApiServiceProvider.GetGroupCount(user.access_token);
    }
  );
  return { ...queryResults };
};
