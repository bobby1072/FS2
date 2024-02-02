import { useQuery } from "react-query";
import Constants from "../../../common/Constants";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

export const useGetGroupCount = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery<number, Error>(
    Constants.QueryKeys.GetGroupCount,
    () => {
      if (!user?.access_token) throw new Error("No bearer token found");
      return BackendApiServiceProvider.GetGroupCount(user.access_token);
    }
  );
  return { ...queryResults };
};
