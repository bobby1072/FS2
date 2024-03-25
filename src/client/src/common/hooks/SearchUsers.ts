import { useMutation } from "react-query";
import { useAuthentication } from "../contexts/AuthenticationContext";
import { UserModel } from "../../models/UserModel";
import { ApiException } from "../ApiException";
import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";

export const useSearchUsers = (minTermLength: number = 2) => {
  const { user } = useAuthentication();
  if (!user) {
    throw new Error("User not found");
  }
  const mutationResult = useMutation<
    Omit<UserModel, "email">[],
    ApiException,
    { searchTerm: string }
  >(async (st) =>
    st.searchTerm.length < minTermLength
      ? []
      : BackendApiServiceProvider.SearchUsers(st.searchTerm, user.access_token)
  );
  return { ...mutationResult };
};
