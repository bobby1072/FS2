import ApiError from "../common/ApiError";
import Constants from "../common/Constants";
import User from "../common/RuntimeTypes/User";
import { UsernamePasswordType } from "../controllers/RequestBodySchema/UsernamePassword";
import UserRepository from "../persistence/Repositories/UserRepository";
import UserRoleRepository from "../persistence/Repositories/UserRoleRepository";
import BaseService from "./BaseService";

export default class UserService extends BaseService<UserRepository> {
  private readonly _userRoleRepo: UserRoleRepository;
  constructor(superRepo: UserRepository, userRoleRepo: UserRoleRepository) {
    super(superRepo);
    this._userRoleRepo = userRoleRepo;
    return this;
  }
  public async EnsureAdminUser(): Promise<User> {
    const foundAdmin = await this._repo.Get("admin@null.null");
    const newAdmin = new User({
      Email: "admin@null.null",
      PasswordHash: process.env.ADMIN_PASSWORD ?? "admin",
      RoleName: "AdminUser",
      Username: "AdminUser123",
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
  public async UpdateUser(newUser: User, userEmail: string): Promise<User> {
    const userExist = await this._repo.Get(userEmail);
    if (!userExist) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    newUser.CreatedAt = userExist.CreatedAt;
    const safeUser = new User(newUser);
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
  public async DeleteUser(user: User | string): Promise<void> {
    const userExist = await this._repo.UserExists({
      username: typeof user === "string" ? user : user.Username,
    });
    if (!userExist) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    const deleted = await this._repo.Delete(user);
    if (!deleted) {
      throw new ApiError(Constants.ExceptionMessages.failedToDeleteUser, 500);
    }
  }
  public async RegisterUser(user: User): Promise<User> {
    const userExist = await this._repo.UserExists({ username: user.Username });
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
  public async LoginUser(user: User | UsernamePasswordType): Promise<User> {
    const userUsername = user instanceof User;
    const foundUser = await this._repo.Get(userUsername ? user : user.Username);
    if (!foundUser) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    if (
      User.isHashedPasswordEqualTo(
        userUsername ? user.PasswordHash : user.Password,
        foundUser.PasswordHash
      )
    ) {
      return foundUser;
    } else {
      throw new ApiError(Constants.ExceptionMessages.inncorrectPassword, 401);
    }
  }
}
