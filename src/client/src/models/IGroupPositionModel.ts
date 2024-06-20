export interface IGroupPositionModel {
  id?: number | null;
  groupId: string;
  name: string;
  canManageGroup: boolean;
  canReadCatches: boolean;
  canManageCatches: boolean;
  canReadMembers: boolean;
  canManageMembers: boolean;
}
