import { UserModel } from "./UserModel";

export interface GroupModel {
  id?: string;
  name: string;
  description?: string;
  leaderEmail: string;
  leader?: UserModel | null;
  createdAt: string;
  public: boolean;
  listed: boolean;
  emblem?: string | null;
}
