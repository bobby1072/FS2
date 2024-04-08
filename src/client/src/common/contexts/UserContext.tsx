import React, { ReactNode, createContext, useContext, useEffect } from "react";
import { IUserWithPermissionsRawModel } from "../../models/IUserModel";
import { useGetUserWithPermissionsQuery } from "../hooks/GetUserQuery";
import { Loading } from "../Loading";
import { group } from "console";

export const UserContext = createContext<
  IUserWithPermissionsRawModel | undefined
>(undefined);

export const useCurrentUser = () => {
  const value = useContext(UserContext);
  if (!value)
    return {
      username: undefined,
      id: undefined,
      email: undefined,
      groupPermissions: {
        abilities: [],
      },
      emailVerified: false,
      name: undefined,
    };
  return value;
};

export const UserContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const { data: userPerms, isLoading } = useGetUserWithPermissionsQuery();
  useEffect(() => {
    if (userPerms) {
      console.log(userPerms);
    }
  }, [userPerms]);
  if (isLoading && !userPerms) return <Loading fullScreen />;
  return (
    <UserContext.Provider value={userPerms}>{children}</UserContext.Provider>
  );
};
