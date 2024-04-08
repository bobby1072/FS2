import { PermissionActions } from "../common/contexts/AbilitiesContext";
import { IGroupModel } from "./IGroupModel";

export interface IUserModel {
  id?: string;
  email: string;
  username: string;
  emailVerified: boolean;
  name?: string | null;
}

export interface IUserWithPermissionsRawModel extends IUserModel {
  groupPermissions: {
    abilities: {
      action: PermissionActions;
      subject: string;
      fields?: string[] | null;
    }[];
  };
}
