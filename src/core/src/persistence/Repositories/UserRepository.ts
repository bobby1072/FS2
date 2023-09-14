import User from "../../common/RuntimeTypes/User";
import UserEntity from "../Entities/UserEntity";
import ApiError from "../../common/ApiError";
import Constants from "../../common/Constants";
import BaseRepository from "./BaseRepository";
export default class UserRepository extends BaseRepository<UserEntity> {
  public async UserExists(username: string): Promise<boolean> {
    const dbUser = await this._repo
      .createQueryBuilder("u")
      .where("u.username = :username", { username: username })
      .getOne();
    if (!dbUser) {
      return false;
    }
    return true;
  }
  public async GetAllUsers(): Promise<User[]> {
    return this._repo
      .find()
      .then((users) => Promise.all(users.map((x) => x.ToRuntimeTypeSync())));
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
    await this._repo.save(await user.ToEntityAsync());
    return this.Get(user);
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
  public async Update(newUser: User, oldUsername: string): Promise<Boolean> {
    return this._repo
      .createQueryBuilder()
      .update(UserEntity)
      .set(await newUser.ToEntityAsync())
      .where("username = :username", { username: oldUsername })
      .execute()
      .then((data) => !!data.affected)
      .catch((error) => {
        throw new ApiError(Constants.ExceptionMessages.failedToUpdateUser, 500);
      });
  }
}
