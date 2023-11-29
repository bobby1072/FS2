import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { useQuery } from "react-query";
import Constants from "../Constants";
import { ClientConfigResponse } from "../../models/ClientConfigResponse";
import { AxiosError } from "axios";
export const useClientConfigQuery = () => {
  const { data, isLoading, error } = useQuery<ClientConfigResponse, AxiosError>(
    Constants.QueryKeys.ClientConfig,
    () => BackendApiServiceProvider.GetClientConfig()
  );
  return {
    data,
    isLoading,
    error,
  };
};
