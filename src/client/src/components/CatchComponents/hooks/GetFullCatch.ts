import { useMutation } from "react-query";
import { ApiException } from "../../../common/ApiException";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { IGroupCatchModel } from "../../../models/IGroupCatchModel";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

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
