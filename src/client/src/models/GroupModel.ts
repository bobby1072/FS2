import { UserModel } from "./UserModel";

export interface GroupModel {
  id?: string;
  name: string;
  description?: string;
  leaderEmail: string;
  leader?: UserModel | null;
  createdAt: string;
  Public: boolean;
  Listed: boolean;
  emblem?: string | null;
}
