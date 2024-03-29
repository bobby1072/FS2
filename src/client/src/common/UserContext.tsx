import React, { ReactNode, createContext, useContext } from "react";
import { IUserModel } from "../models/UserModel";
import { useGetUserQuery } from "./hooks/GetUserQuery";
import { Loading } from "./Loading";

export const UserContext = createContext<IUserModel | undefined>(undefined);

export const useCurrentUser = () => {
  const value = useContext(UserContext);
  if (!value)
    return {
      username: undefined,
      id: undefined,
      email: undefined,
      emailVerified: false,
      name: undefined,
    };
  return value;
};

export const UserContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const { data, isLoading } = useGetUserQuery();
  if (isLoading && !data) return <Loading fullScreen />;
  return <UserContext.Provider value={data}>{children}</UserContext.Provider>;
};
