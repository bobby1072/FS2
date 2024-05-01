import { useMutation, useQuery } from "react-query";
import { ApiException } from "../../../common/ApiException";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { IGroupCatchModel } from "../../../models/IGroupCatchModel";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";
import Constants from "../../../common/Constants";

export const useGetFullCatchMutation = () => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const mutationResults = useMutation<IGroupCatchModel, ApiException, string>(
    (catchId) =>
      BackendApiServiceProvider.GetFullCatchById(catchId, user.access_token),
    {
      retry: (fc) => fc < 1,
    }
  );
  return { ...mutationResults };
};

export const useGetFullCatchQuery = (catchId?: string) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<IGroupCatchModel, ApiException>(
    Constants.QueryKeys.GetFullCatch,
    () => {
      if (!user) throw new Error("User is not authenticated");
      if (!catchId) throw new ApiException("No group id given");
      return BackendApiServiceProvider.GetFullCatchById(
        catchId,
        user.access_token
      );
    }
  );
  return { ...queryResults };
};
