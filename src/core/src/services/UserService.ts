import ApiError from "../common/ApiError";
import Constants from "../common/Constants";
import TokenData from "../common/RuntimeTypes/TokenData";
import User from "../common/RuntimeTypes/User";
import { UsernamePasswordType } from "../controllers/RequestBodySchema/UsernamePassword";
import UserEntity from "../persistence/Entities/UserEntity";
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
    const newAdmin = new User({
      Email: "admin@null.null",
      PasswordHash: process.env.ADMIN_PASSWORD ?? "admin",
      RoleName: Constants.UserRoleNames.admin,
      Username: "AdminUser123",
      Verified: true,
      CreatedAt: new Date(),
    });
    newAdmin.HashPassword();
    const dbAdmin = await this._repo.Create(newAdmin);
    if (!dbAdmin) {
      throw new Error(Constants.ExceptionMessages.failedToCreateAdmin);
    }
    return dbAdmin;
  }
  public async UpdateUser(
    newUser: User,
    username: string,
    options: { updateUsername?: boolean; existingUser?: User } = {
      updateUsername: undefined,
      existingUser: undefined,
    }
  ): Promise<User> {
    const userExist = options.existingUser
      ? options.existingUser
      : await this._repo.Get(username);
    if (!userExist) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    if (!options.updateUsername) {
      newUser.Username = userExist.Username;
    }
    if (newUser.PasswordHash !== userExist.PasswordHash) {
      newUser.HashPassword();
    }
    newUser.CreatedAt = userExist.CreatedAt;
    newUser.RoleName = userExist.RoleName;
    const safeUser = new User(newUser);

    const updated = options.updateUsername
      ? await this._repo.UpdatePrimaryKeyOfRecord(
          userExist.Username,
          safeUser,
          UserEntity,
          "username"
        )
      : await this._repo.Create(safeUser);
    const createdDbUser =
      updated instanceof User
        ? updated
        : await this._repo.Get(safeUser.Username);
    if (!createdDbUser) {
      throw new ApiError(Constants.ExceptionMessages.failedToUpdateUser, 500);
    }
    return createdDbUser;
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
    const userExist = await this._repo.UserUnique({
      username: user.Username,
      email: user.Email,
      phoneNumber: user.PhoneNumber ? user.PhoneNumber : undefined,
    });
    if (userExist) {
      throw new ApiError(Constants.ExceptionMessages.userAlreadyExists, 403);
    }
    user.CreatedAt = new Date();
    user.RoleName = Constants.UserRoleNames.standardUser;
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
  public async LoginUserFromTokenWithoutPassword(
    user: TokenData
  ): Promise<User> {
    const foundUser = await this._repo.Get(user.user);
    if (!foundUser) {
      throw new ApiError(Constants.ExceptionMessages.noUserFound, 404);
    }
    return foundUser;
  }
}
