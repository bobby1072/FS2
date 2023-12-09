import { useQuery } from "react-query";
import { useAuthentication } from "../../../common/login/Authentication";
import { GroupModel } from "../../../models/GroupModel";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

export const useGetAllListedGroups = () => {
  const { bearerToken } = useAuthentication();
  const queryResults = useQuery<GroupModel[], Error>(
    Constants.QueryKeys.GetAllListedGroups,
    () => {
      if (!bearerToken) throw new Error("No bearer token found");
      return BackendApiServiceProvider.GetAllListedGroups(bearerToken);
    }
  );
  return { ...queryResults };
};
