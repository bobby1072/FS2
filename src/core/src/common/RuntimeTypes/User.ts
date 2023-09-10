import { compareSync, genSaltSync, hashSync } from "bcryptjs";
import { UserSchema, UserType } from "./Schemas/UserSchema";
import { sign, verify } from "jsonwebtoken";
import ApiError from "../ApiError";
import Constants from "../Constants";
import TokenData from "./TokenData";
import UserEntity from "../../persistence/Entities/UserEntity";
import BaseRuntime from "./BaseRuntime";
export default class User extends BaseRuntime implements UserType {
  public Email: string;
  private static readonly _schema = UserSchema;
  public PasswordHash: string;
  public PhoneNumber?: string | null;
  public CreatedAt: Date;
  constructor({
    email,
    pass,
    phoneNum,
    createdAt,
  }: {
    email: string;
    pass: string;
    phoneNum?: string | null;
    createdAt?: Date;
  }) {
    super();
    if (!createdAt) {
      createdAt = new Date();
    }
    const { Email, PasswordHash, PhoneNumber, CreatedAt } = User._schema.parse({
      PhoneNumber: phoneNum,
      Email: email,
      PasswordHash: pass,
      CreatedAt: createdAt,
    });
    this.Email = Email;
    this.CreatedAt = CreatedAt;
    this.PhoneNumber = PhoneNumber;
    this.PasswordHash = PasswordHash;
    return this;
  }
  public static EncodeToken(email: string): string {
    return sign(
      {
        user: email,
      },
      process.env.SK ?? "dev_secret_key",
      { algorithm: "HS256", expiresIn: "1h" }
    );
  }
  public static DecodeToken(token: string): TokenData {
    try {
      if (token.includes("Bearer ")) token = token.replace("Bearer ", "");
      const decodedToken = verify(
        token,
        process.env.SK ?? "dev_secret_key"
      ) as any;
      return new TokenData(
        decodedToken.user,
        decodedToken.iat,
        decodedToken.exp
      );
    } catch (e) {
      throw new ApiError(Constants.ExceptionMessages.invalidToken, 401);
    }
  }
  public ToEntity(): UserEntity {
    return UserEntity.ParseSync(this._toJson());
  }
  public async ToEntityAsync(): Promise<UserEntity> {
    return UserEntity.ParseAsync(this._toJson());
  }
  public HashPassword(): string {
    this.PasswordHash = hashSync(this.PasswordHash, genSaltSync());
    return this.PasswordHash;
  }
  public static isHashedPasswordEqualTo(
    stringPass: string,
    passHash: string
  ): boolean {
    return compareSync(stringPass, passHash);
  }
}
