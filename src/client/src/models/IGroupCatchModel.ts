import { IGroupModel } from "./IGroupModel";
import { IUserModel } from "./IUserModel";
import { IWorldFishModel } from "./IWorldFishModel";

export interface IPartialGroupCatchModel {
  id: string;
  species: string;
  latitude: number;
  longitude: number;
  caughtAt: string;
  weight: number;
  length: number;
  user: Omit<IUserModel, "email">;
  worldFish?: IWorldFishModel | null;
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

export class RuntimePartialGroupCatchModel
  implements Omit<IPartialGroupCatchModel, "caughtAt">
{
  public id: string;
  public species: string;
  public latitude: number;
  public longitude: number;
  public caughtAt: Date;
  public weight: number;
  public length: number;
  public user: Omit<IUserModel, "email">;
  public worldFish?: IWorldFishModel | null;
  public constructor(rawPartialGroupCatchModel: IPartialGroupCatchModel) {
    this.id = rawPartialGroupCatchModel.id;
    this.species = rawPartialGroupCatchModel.species;
    this.latitude = rawPartialGroupCatchModel.latitude;
    this.longitude = rawPartialGroupCatchModel.longitude;
    this.caughtAt = new Date(rawPartialGroupCatchModel.caughtAt);
    this.weight = rawPartialGroupCatchModel.weight;
    this.length = rawPartialGroupCatchModel.length;
    this.user = rawPartialGroupCatchModel.user;
    this.worldFish = rawPartialGroupCatchModel.worldFish;
  }
}
// export class RuntimeGroupCatchModel
//   implements Omit<IGroupCatchModel, "caughtAt">
// {
//   public id: string;
//   public groupId: string;
//   public species: string;
//   public worldFishTaxocode?: string | null;
//   public worldFish?: IWorldFishModel | null;
//   public weight: number;
//   public length: number;
//   public description?: string | null;
//   public latitude: number;
//   public longitude: number;
//   public createdAt: string;
//   public userId: string;
//   public user?: IUserModel | null;
//   public catchPhoto?: string | null;
//   public caughtAt: Date;
//   public constructor(rawGroupCatchModel: IGroupCatchModel) {
//     this.id = rawGroupCatchModel.id;
//     this.groupId = rawGroupCatchModel.groupId;
//     this.species = rawGroupCatchModel.species;
//     this.worldFishTaxocode = rawGroupCatchModel.worldFishTaxocode;
//     this.worldFish = rawGroupCatchModel.worldFish;
//     this.weight = rawGroupCatchModel.weight;
//     this.length = rawGroupCatchModel.length;
//     this.description = rawGroupCatchModel.description;
//     this.latitude = rawGroupCatchModel.latitude;
//     this.longitude = rawGroupCatchModel.longitude;
//     this.caughtAt = new Date(rawGroupCatchModel.caughtAt);
//     this.createdAt = rawGroupCatchModel.createdAt;
//     this.userId = rawGroupCatchModel.userId;
//     this.user = rawGroupCatchModel.user;
//     this.catchPhoto = rawGroupCatchModel.catchPhoto;
//   }
// }
