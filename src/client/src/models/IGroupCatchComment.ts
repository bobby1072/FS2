import { IUserWithoutEmailModel } from "./IUserModel";

export interface IGroupCatchCommentTaggedUsersModel {
  id?: number;
  commentId: number;
  userId: string;
  user?: IUserWithoutEmailModel;
}

export interface IGroupCatchCommentModel {
  id?: number;
  groupCatchId: string;
  userId: string;
  user?: IUserWithoutEmailModel;
  comment: string;
  createdAt: string;
  taggedUsers?: IGroupCatchCommentTaggedUsersModel[];
}
