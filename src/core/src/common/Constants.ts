enum ExceptionMessages {
  internalServerError = "Internal server error",
  invalidToken = "Invalid token or no token",
  noUserFound = "No user with that email found",
  inncorrectPassword = "Inncorrect password",
  userAlreadyExists = "User already exists",
  emailOrPasswordNotIncluded = "Email or password not included",
  failedToDeleteUser = "Failed to delete user",
  failedToUpdateUser = "Failed to update user",
  failedToCreateUser = "Failed to create user",
  passwordEmptyOrInvalid = "You need to enter a valid password",
  emailEmpty = "You need to enter a email",
  inncorrectPhoneFormat = "Inncorect phone format",
  tokenExpired = "Auth token is expired",
  failedToRegisterJobs = "Failed to register scheduled jobs",
  invalidOrEmptyUsername = "No username or invalid username",
  missingEmailOrUsername = "Missing email or username",
}
enum Buzzwords {
  read = "Read",
  create = "Create",
  update = "Update",
  delete = "Delete",
  none = "None",
}
enum UserRoleNames {
  admin = "AdminUser",
  standardUser = "StandardUser",
}
const AdminRole = {
  RoleName: "AdminUser",
  GroupPermissions: [
    Buzzwords.create,
    Buzzwords.read,
    Buzzwords.update,
    Buzzwords.delete,
  ],
};
const StandardRole = {
  RoleName: "StandardUser",
  GroupPermissions: [Buzzwords.read],
};
export default abstract class Constants {
  public static readonly ExceptionMessages: typeof ExceptionMessages =
    ExceptionMessages;
  public static readonly PermissionConstants: typeof Buzzwords = Buzzwords;
  public static readonly UserRoleNames: typeof UserRoleNames = UserRoleNames;
  public static readonly UserRoles = {
    standardRole: StandardRole,
    adminRole: AdminRole,
  };
}
