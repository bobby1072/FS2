import { createContext, useContext } from "react";
import { IUserWithPermissionsRawModel } from "../../models/IUserModel";
import { ApiException } from "../ApiException";
import { useCurrentUser } from "./UserContext";
export enum PermissionActions {
  Read = "read",
  Manage = "manage",
  BelongsTo = "belongsTo",
}
export class PermissionProvider {
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
    fields?: string | null
  ) {
    if (fields) {
      return this._permissions.some(
        (p) =>
          (p.action === action &&
            p.subject === subject &&
            (!fields || fields.length === 0)) ||
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

export const AppAbilityContext = createContext<PermissionProvider | undefined>(
  undefined
);

export const useGetPermissionSet = () => {
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
  const permissionSet = new PermissionProvider(abilities);
  return (
    <AppAbilityContext.Provider value={permissionSet}>
      {children}
    </AppAbilityContext.Provider>
  );
};
