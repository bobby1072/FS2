import { IUserModel } from "./IUserModel";

export interface IGroupCatchCommentTaggedUsersModel {
  id?: number;
  commentId: number;
  userId: string;
}

export interface IGroupCatchCommentModel {
  id?: number;
  groupCatchId: string;
  userId: string;
  user?: Omit<IUserModel, "email">;
  comment: string;
  createdAt: Date;
  taggedUsers?: IGroupCatchCommentTaggedUsersModel[];
}
