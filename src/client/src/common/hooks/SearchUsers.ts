import { useMutation } from "react-query";
import { useAuthentication } from "../contexts/AuthenticationContext";
import { IUserWithoutEmailModel } from "../../models/IUserModel";
import { ApiException } from "../ApiException";
import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";

export const useSearchUsers = (minTermLength: number = 2) => {
  const { user } = useAuthentication();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationResult = useMutation<
    IUserWithoutEmailModel[],
    ApiException,
    { searchTerm: string }
  >(async (st) =>
    st.searchTerm.length < minTermLength
      ? []
      : BackendApiServiceProvider.SearchUsers(st.searchTerm, user.access_token)
  );
  return { ...mutationResult };
};
