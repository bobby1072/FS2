import { createContext, useContext } from "react";
import { ClientConfigResponse } from "../../models/ClientConfigResponse";
import { useClientConfigQuery } from "../queries/ClientConfigQuery";
import { Loading } from "../Loading";
import { ApiException } from "../ApiException";

export const AppContext = createContext<ClientConfigResponse | undefined>(
  undefined
);

export const useAppContext = () => {
  const value = useContext(AppContext);
  if (!value) throw new ApiException("AppContext has not been registered");
  return value;
};

export const AppContextProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { data } = useClientConfigQuery();
  if (!data) return <Loading fullScreen />;
  return <AppContext.Provider value={data}>{children}</AppContext.Provider>;
};
