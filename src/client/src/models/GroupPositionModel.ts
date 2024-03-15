export interface GroupPositionModel {
  id?: string;
  groupId: string;
  name: string;
  canManageGroup: boolean;
  canReadCatches: boolean;
  canManageCatches: boolean;
  canReadMembers: boolean;
  canManageMembers: boolean;
}
