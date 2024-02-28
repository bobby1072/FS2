import { GroupModel } from "./GroupModel";

export interface GroupPositionModel {
    id?: number;
    groupId: string;
    group?: GroupModel | null;
    name: string;
    canManageGroup: boolean;
    canReadCatches: boolean;
    canManageCatches: boolean;
    canReadMembers: boolean;
    canManageMembers: boolean;
}