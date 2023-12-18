import { UserModel } from "./UserModel";

export interface GroupModel {
  id?: string;
  name: string;
  description?: string;
  leaderEmail: string;
  leader?: UserModel | null;
  createdAt: string;
  isPublic: boolean;
  isListed: boolean;
  emblem?: Uint8Array | null;
}