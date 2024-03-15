import { GroupModel } from "./GroupModel";
import { GroupPositionModel } from "./GroupPositionModel";

export interface GroupMemberModel {
  id?: string;
  groupId: string;
  group?: GroupModel | null;
  userId: string;
  user?: {
    id: string;
    email?: string | null;
    username: string;
    emailVerified: boolean;
    name?: string | null;
  } | null;
  positionId: string;
  position?: GroupPositionModel | null;
}
