import { IGroupModel } from "./IGroupModel";
import { IUserModel } from "./IUserModel";
import { IWorldFishModel } from "./IWorldFishModel";

export interface IPartialGroupCatchModel {
  Species: string;
  Latitude: number;
  Longitude: number;
}

export interface IGroupCatchModel {
  id: string;
  groupId: string;
  group?: IGroupModel | null;
  species: string;
  worldFishTaxocode?: string | null;
  worldFish?: IWorldFishModel | null;
  weight: number;
  length: number;
  description?: string | null;
  latitude: number;
  longitude: number;
  caughtAt: string;
  createdAt: string;
  userId: string;
  user?: IUserModel | null;
  catchPhoto?: string | null;
}
