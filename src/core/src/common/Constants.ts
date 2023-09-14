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
  passwordEmpty = "You need to enter a password",
  emailEmpty = "You need to enter a email",
  inncorrectPhoneFormat = "Inncorect phone format",
  tokenExpired = "Auth token is expired",
  failedToRegisterJobs = "Failed to register scheduled jobs",
  invalidOrEmptyUsername = "No username or invalid username",
  missingEmailOrUsername = "Missing email or username",
}
export default abstract class Constants {
  public static readonly ExceptionMessages: typeof ExceptionMessages =
    ExceptionMessages;
}
