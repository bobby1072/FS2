import React, { ReactNode, createContext, useContext } from "react";
import { UserModel } from "../models/UserModel";
import { useGetUserQuery, useGetUserQueryConstantRefresh } from "./queries/GetUserQuery";
import { Loading } from "./Loading";
import { ApiException } from "./ApiException";

export const UserContext = createContext<UserModel | undefined>(undefined);

export const useCurrentUser = () => {
  const value = useContext(UserContext);
  if (!value) throw new ApiException("UserContext has not been registered");
  return value;
};

export const UserContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const { data, isLoading } = useGetUserQuery();
  if (isLoading && !data) return <Loading fullScreen />;
  return <UserContext.Provider value={data}>{children}</UserContext.Provider>;
};

export const UserContextProviderWithConstantRefresh: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const { data, isLoading } = useGetUserQueryConstantRefresh();
  if (isLoading && !data) return <Loading fullScreen />;
  return <UserContext.Provider value={data}>{children}</UserContext.Provider>;
};
