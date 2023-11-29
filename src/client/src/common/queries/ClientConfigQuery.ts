import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { useQuery } from "react-query";
import Constants from "../Constants";
export const useClientConfigQuery = () => {
  const { data, isLoading, error } = useQuery(
    Constants.ClientConfigQueryKey,
    () => BackendApiServiceProvider.GetClientConfig()
  );
  return {
    data,
    isLoading,
    error,
  };
};
