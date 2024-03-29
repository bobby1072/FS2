export interface IUserModel {
  id?: string;
  email: string;
  username: string;
  emailVerified: boolean;
  name?: string | null;
}
