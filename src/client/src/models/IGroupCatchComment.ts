import { IUserModel } from "./IUserModel";

export interface IGroupCatchCommentTaggedUsersModel {
  id?: number;
  commentId: number;
  userId: string;
  user?: Omit<IUserModel, "email">;
}

export interface IGroupCatchCommentModel {
  id?: number;
  groupCatchId: string;
  userId: string;
  user?: Omit<IUserModel, "email">;
  comment: string;
  createdAt: string;
  taggedUsers?: IGroupCatchCommentTaggedUsersModel[];
}