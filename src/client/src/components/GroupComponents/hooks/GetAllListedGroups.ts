import { useQuery } from "react-query";
import { GroupModel } from "../../../models/GroupModel";
import Constants from "../../../common/Constants";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";

export const useGetAllListedGroups = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery<GroupModel[], Error>(
    Constants.QueryKeys.GetAllListedGroups,
    () => {
      if (!user?.id_token) throw new Error("No bearer token found");
      return BackendApiServiceProvider.GetAllListedGroups(user.id_token);
    }
  );
  return { ...queryResults };
};
