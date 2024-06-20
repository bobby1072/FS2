import { IGroupModel } from "./IGroupModel";
import { IGroupPositionModel } from "./IGroupPositionModel";

export interface IGroupMemberModel {
  id?: number | null;
  groupId: string;
  group?: IGroupModel | null;
  userId: string;
  user?: {
    id: string;
    email?: string | null;
    username: string;
    emailVerified: boolean;
    name?: string | null;
  } | null;
  positionId: number;
  position?: IGroupPositionModel | null;
}
