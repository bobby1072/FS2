import { useMutation } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

export const useSaveCatchMutation = () => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const mutationResults = useMutation<string, ApiException, FormData>((gc) =>
    BackendApiServiceProvider.SaveGroupCatch(gc, user.access_token)
  );
  return { ...mutationResults };
};
