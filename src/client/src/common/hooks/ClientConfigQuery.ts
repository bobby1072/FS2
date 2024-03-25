import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { useQuery } from "react-query";
import Constants from "../Constants";
import { ClientConfigResponse } from "../../models/ClientConfigResponse";
import { ApiException } from "../ApiException";
export const useClientConfigQuery = () => {
  const queryResults = useQuery<ClientConfigResponse, ApiException>(
    Constants.QueryKeys.ClientConfig,
    () => BackendApiServiceProvider.GetClientConfig()
  );
  return {
    ...queryResults,
  };
};
