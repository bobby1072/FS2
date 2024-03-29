import { IGroupModel } from "./GroupModel";
import { IGroupPositionModel } from "./GroupPositionModel";

export interface IGroupMemberModel {
  id?: string;
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
  positionId: string;
  position?: IGroupPositionModel | null;
}
