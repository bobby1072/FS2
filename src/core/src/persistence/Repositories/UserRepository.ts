import User from "../../common/RuntimeTypes/User";
import UserEntity from "../Entities/UserEntity";
import ApiError from "../../common/ApiError";
import Constants from "../../common/Constants";
import BaseRepository from "./BaseRepository";
export default class UserRepository extends BaseRepository<UserEntity, User> {
  public async UserUnique({
    email,
    username,
    phoneNumber,
  }: {
    email: string;
    username: string;
    phoneNumber?: string;
  }) {
    return this._repo
      .createQueryBuilder("u")
      .where("u.username = :username", { username })
      .orWhere("u.email = :email", { email })
      .orWhere("u.phone_number = :phoneNumber", { phoneNumber })
      .getOne()
      .then((x) => !!x);
  }
  public async UserExists({
    username,
  }: {
    username?: string;
  }): Promise<boolean> {
    if (!username) {
      throw new ApiError(Constants.ExceptionMessages.missingEmailOrUsername);
    }
    const dbUser = await this._repo
      .createQueryBuilder("u")
      .where("u.username = :username", { username })
      .getOne();
    if (!dbUser) {
      return false;
    }
    return true;
  }
  public async Get(
    user: User | string,
    options: { includeUserRole: boolean } = { includeUserRole: false }
  ): Promise<User | undefined> {
    return this._repo
      .findOne({
        where: {
          Username: typeof user === "string" ? user : user.Username,
        },
        relations: {
          Role: options.includeUserRole,
        },
      })
      .then((dbUser) => dbUser?.ToRuntimeTypeAsync());
  }
  public async Create(user: User): Promise<User | undefined> {
    return (
      await this._repo.save(await user.ToEntityAsync())
    ).ToRuntimeTypeAsync();
  }
  public async Delete(user: string | User): Promise<boolean> {
    return this._repo
      .createQueryBuilder()
      .delete()
      .where("username = :username", {
        username: typeof user === "string" ? user : user.Username,
      })
      .from("user")
      .execute()
      .then((data) => true)
      .catch((error) => {
        throw new ApiError(Constants.ExceptionMessages.failedToDeleteUser, 500);
      });
  }
}
