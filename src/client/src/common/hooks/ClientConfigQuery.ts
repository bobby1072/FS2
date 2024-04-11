import BackendApiServiceProvider from "../../utils/BackendApiServiceProvider";
import { useQuery } from "react-query";
import Constants from "../Constants";
import { IClientConfigResponse } from "../../models/IClientConfigResponse";
import { ApiException } from "../ApiException";
export const useClientConfigQuery = () => {
  const queryResults = useQuery<IClientConfigResponse, ApiException>(
    Constants.QueryKeys.ClientConfig,
    () => BackendApiServiceProvider.GetClientConfig(),
    {
      retry: (count) => count < 2,
    }
  );
  return {
    ...queryResults,
  };
};
