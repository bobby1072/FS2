import User from "../../common/RuntimeTypes/User";
import UserEntity from "../Entities/UserEntity";
import ApiError from "../../common/ApiError";
import Constants from "../../common/Constants";
import BaseRepository from "./BaseRepository";
import { SelectQueryBuilder } from "typeorm";
type UserTypeExtens = boolean;
type ObjectType<T> = T extends true ? UserEntity | undefined : User | undefined;
export default class UserRepository extends BaseRepository<UserEntity, User> {
  public async UserExists({
    username,
    email,
  }: {
    username?: string;
    email?: string;
  }): Promise<boolean> {
    if (!email && !username) {
      throw new ApiError(Constants.ExceptionMessages.missingEmailOrUsername);
    }
    const tempFunc = (x: SelectQueryBuilder<UserEntity>) => {
      if (username) {
        x = x.where("u.username = :username", { username: username });
        if (email) {
          x = x.andWhere("u.email = :email", { email: email });
        }
      } else if (email) {
        x = x.where("u.email = :email", { email: email });
      }
      return x;
    };
    const dbUser = await tempFunc(this._repo.createQueryBuilder("u")).getOne();
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
      .then((data) => !!data.affected)
      .catch((error) => {
        throw new ApiError(Constants.ExceptionMessages.failedToDeleteUser, 500);
      });
  }
}
