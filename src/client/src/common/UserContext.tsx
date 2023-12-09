import React, { ReactNode, createContext, useContext } from "react";
import { UserModel } from "../models/UserModel";
import { useGetUserQuery } from "./queries/GetUserQuery";
import { Loading } from "./Loading";

export const UserContext = createContext<UserModel | undefined>(undefined);

export const useCurrentUser = () => {
  const value = useContext(UserContext);
  if (!value) throw new Error("UserContext has not been registered");
  return value;
};

export const UserContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const { data } = useGetUserQuery();
  if (!data) return <Loading fullScreen />;
  return <UserContext.Provider value={data}>{children}</UserContext.Provider>;
};
