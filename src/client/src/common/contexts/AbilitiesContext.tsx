import { createContext, useContext, useEffect, useState } from "react";
import { IUserWithPermissionsRawModel } from "../../models/IUserModel";
import { ApiException } from "../ApiException";
import { useCurrentUser } from "./UserContext";
export enum PermissionActions {
  Read = "read",
  Manage = "manage",
  BelongsTo = "belongsTo",
}
export enum PermissionFields {
  GroupCatch = "GroupCatch",
  GroupMember = "GroupMember",
}
export class PermissionManager {
  private _permissions: IUserWithPermissionsRawModel["groupPermissions"]["abilities"] =
    [];
  public constructor(
    permissionSet: IUserWithPermissionsRawModel["groupPermissions"]["abilities"]
  ) {
    this._permissions = permissionSet;
  }
  public Can(
    action: PermissionActions,
    subject: string,
    fields?: PermissionFields | null
  ) {
    if (fields) {
      return this._permissions.some(
        (p) =>
          (p.action === action &&
            p.subject === subject &&
            (!p.fields || p.fields.length === 0)) ||
          (p.action === action &&
            p.subject === subject &&
            p.fields?.includes(fields))
      );
    }
    return this._permissions.some(
      (p) => p.action === action && p.subject === subject
    );
  }
}

export const AppAbilityContext = createContext<
  { permissionManager: PermissionManager } | undefined
>(undefined);

export const useCurrentPermissionSet = () => {
  const value = useContext(AppAbilityContext);
  if (!value) throw new ApiException("No permission set found");
  return value;
};

export const PermissionContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const {
    groupPermissions: { abilities },
  } = useCurrentUser();
  const [permissionManager] = useState<PermissionManager>(
    new PermissionManager(abilities ?? [])
  );
  return (
    <AppAbilityContext.Provider value={{ permissionManager }}>
      {children}
    </AppAbilityContext.Provider>
  );
};
