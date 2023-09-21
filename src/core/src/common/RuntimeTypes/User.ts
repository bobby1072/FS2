import { compareSync, genSaltSync, hashSync } from "bcryptjs";
import { UserSchema, UserType } from "./Schemas/UserSchema";
import { sign, verify } from "jsonwebtoken";
import ApiError from "../ApiError";
import Constants from "../Constants";
import TokenData from "./TokenData";
import UserEntity from "../../persistence/Entities/UserEntity";
import BaseRuntime from "./BaseRuntime";
import UserRole from "./UserRole";
import { DeepPartial } from "../DeepPartial";
export default class User extends BaseRuntime implements UserType {
  private static readonly _schema = UserSchema;
  public Email: string;
  public Username: string;
  public Name?: string | null;
  public Description?: string | null;
  public Verified: boolean;
  public RoleName: string;
  public Role?: UserRole | null;
  public PasswordHash: string;
  public PhoneNumber?: string | null;
  public CreatedAt: Date;
  constructor({
    Email: email,
    PasswordHash: pass,
    PhoneNumber: phoneNum,
    CreatedAt: createdAt = new Date(),
    Username: username,
    Name: name,
    Description: description,
    Verified: verified = false,
    RoleName: roleName,
    Role: role,
  }: DeepPartial<UserType> & { Role?: UserRole | null }) {
    super();
    const {
      Email,
      PasswordHash,
      PhoneNumber,
      CreatedAt,
      RoleName,
      Username,
      Verified,
      Description,
      Name,
    } = User._schema.parse({
      PhoneNumber: phoneNum,
      Email: email,
      PasswordHash: pass,
      CreatedAt: createdAt,
      RoleName: roleName,
      Username: username,
      Verified: verified,
      Description: description,
      Name: name,
    });
    this.Email = Email;
    this.CreatedAt = CreatedAt;
    this.PhoneNumber = PhoneNumber;
    this.PasswordHash = PasswordHash;
    this.Description = Description;
    this.Role = role;
    this.RoleName = RoleName;
    this.Username = Username;
    this.Verified = Verified;
    this.Name = Name;
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
