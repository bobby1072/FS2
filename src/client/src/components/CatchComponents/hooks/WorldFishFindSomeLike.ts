import { useMutation } from "react-query";
import { useAuthentication } from "../../../common/contexts/AuthenticationContext";
import { IWorldFishModel } from "../../../models/IWorldFishModel";
import { ApiException } from "../../../common/ApiException";
import BackendApiServiceProvider from "../../../utils/BackendApiServiceProvider";

export const useWorldFishFindSomeLikeMutation = () => {
  const { user } = useAuthentication();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationResults = useMutation<
    IWorldFishModel[],
    ApiException,
    { fishAnyname: string }
  >(
    (f) =>
      BackendApiServiceProvider.WorldFishClient.FindSomeLink(
        f.fishAnyname,
        user.access_token
      ),
    {
      retry: (_, e) => (e.status === 404 ? false : true),
    }
  );
  return { ...mutationResults };
};
