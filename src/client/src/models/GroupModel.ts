import { GroupMemberModel } from "./GroupMemberModel";
import { GroupPositionModel } from "./GroupPositionModel";

export interface GroupModel {
  id?: string;
  name: string;
  description?: string | null;
  leaderUsername: string;
  leader?:  {
    email?: string | null;
    username: string;
    emailVerified: boolean;
    name?: string | null;
  } | null;
  createdAt: string;
  public: boolean;
  listed: boolean;
  emblem?: string | null;
  members?: GroupMemberModel[] | null;
  positions?: GroupPositionModel[] | null;
}
