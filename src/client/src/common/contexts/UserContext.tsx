import React, {
  ReactNode,
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";
import { IUserWithPermissionsRawModel } from "../../models/IUserModel";
import { useGetUserWithPermissionsQuery } from "../hooks/GetUserQuery";
import { Loading } from "../Loading";

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
        abilities: undefined,
      },
      emailVerified: false,
      name: undefined,
    };
  return value;
};

export const UserContextProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [userPermState, setUserPermState] =
    useState<IUserWithPermissionsRawModel>();
  const { data, isLoading } = useGetUserWithPermissionsQuery();
  useEffect(() => {
    setUserPermState(data);
  }, [data]);
  if (isLoading && !userPermState) return <Loading fullScreen />;
  return (
    <UserContext.Provider value={userPermState}>
      {children}
    </UserContext.Provider>
  );
};
