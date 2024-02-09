export interface UserModel {
  email: string;
  username: string;
  emailVerified: boolean;
  name?: string | null;
}
