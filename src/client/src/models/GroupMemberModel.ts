import { GroupModel } from "./GroupModel";
import { GroupPositionModel } from "./GroupPositionModel";

export interface GroupMemberModel {
    id?: number;
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
    positionId: number;
    position?: GroupPositionModel | null;
}