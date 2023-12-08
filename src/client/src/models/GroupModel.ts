import { UserModel } from "./UserModel";

export interface GrouopModel {
  id?: string;
  name: string;
  description?: string;
  leaderEmail: string;
  leader?: UserModel | null;
  createdAt: string;
  isPublic: boolean;
  isListed: boolean;
  emblem?: number[] | null;
}
