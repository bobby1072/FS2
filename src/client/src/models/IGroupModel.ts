import { IGroupPositionModel } from "./IGroupPositionModel";

export interface IGroupModel {
  id?: string;
  name: string;
  description?: string | null;
  leaderId: string;
  leader?: {
    id: string;
    email?: string | null;
    username: string;
    emailVerified: boolean;
    name?: string | null;
  } | null;
  createdAt: string;
  public: boolean;
  listed: boolean;
  emblem?: string | null;
  positions?: IGroupPositionModel[] | null;
}
