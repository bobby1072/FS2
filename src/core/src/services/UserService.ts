import { appendFile } from "fs/promises";
import ApiError from "../common/ApiError";
import Constants from "../common/Constants";
import User from "../common/RuntimeTypes/User";
import UserRepository from "../persistence/Repositories/UserRepository";
import BaseService from "./BaseService";

export default class UserService extends BaseService<UserRepository> {
  public async EnsureAdminUser(): Promise<User> {
    const foundAdmin = await this._repo.Get("admin@null.null");
    const newAdmin = new User({
      email: "admin@null.null",
      pass: process.env.ADMIN_PASSWORD ?? "admin",
    });
    if (!foundAdmin) {
      const dbAdmin = await this._repo.Create(newAdmin);
      if (!dbAdmin) {
        throw new ApiError(Constants.ExceptionMessages.failedToCreateUser, 500);
      }
      return dbAdmin;
    } else {
      return this.UpdateUser(newAdmin, "admin@null.null");
    }
  }
  public async LoginUser(user: User): Promise<User> {
    const foundUser = await this._repo.Get(user);
    if (!foundUser) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    if (
      User.isHashedPasswordEqualTo(user.PasswordHash, foundUser.PasswordHash)
    ) {
      return foundUser;
    } else {
      throw new ApiError(Constants.ExceptionMessages.inncorrectPassword, 401);
    }
  }
  public async RegisterUser(user: User): Promise<User> {
    const userExist = await this._repo.UserExists(user.Email);
    if (userExist) {
      throw new ApiError(Constants.ExceptionMessages.userAlreadyExists, 403);
    }
    user.HashPassword();
    const dbNewUser = await this._repo.Create(user);
    if (!dbNewUser) {
      throw new ApiError(Constants.ExceptionMessages.failedToCreateUser, 500);
    }
    return dbNewUser;
  }
  public async DeleteUser(user: User | string): Promise<void> {
    const userExist = await this._repo.UserExists(
      typeof user === "string" ? user : user.Email
    );
    if (!userExist) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    const deleted = await this._repo.Delete(user);
    if (!deleted) {
      throw new ApiError(Constants.ExceptionMessages.failedToDeleteUser, 500);
    }
  }
  public async UpdateUser(newUser: User, userEmail: string): Promise<User> {
    const userExist = await this._repo.Get(userEmail);
    if (!userExist) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    const safeUser = new User({
      email: newUser.Email,
      pass: newUser.PasswordHash,
      createdAt: userExist.CreatedAt,
      phoneNum: newUser.PhoneNumber,
    });
    safeUser.HashPassword();
    const updated = await this._repo.Update(safeUser, userEmail);
    if (!updated) {
      throw new ApiError(Constants.ExceptionMessages.failedToUpdateUser, 500);
    }
    const dbUser = await this._repo.Get(newUser.Email);
    if (!dbUser) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    return dbUser;
  }
}
