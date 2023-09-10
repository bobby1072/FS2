import { Column, Entity, PrimaryColumn } from "typeorm";
import { UserDBSchema, UserDbType } from "../Schemas/UserSchema";
import User from "../../common/RuntimeTypes/User";
import { BaseEntity } from "./BaseEntity";

@Entity({ name: "user" })
export default class UserEntity extends BaseEntity implements UserDbType {
  private static readonly _schema = UserDBSchema;
  @PrimaryColumn({ type: "text" })
  Email!: string;

  @Column({ type: "text", nullable: true })
  PhoneNumber?: string | null;

  @Column({ type: "text" })
  PasswordHash!: string;

  @Column({ type: "timestamptz", default: () => "CURRENT_TIMESTAMP" })
  CreatedAt!: Date;

  public ToRuntimeTypeSync(): User {
    return new User({
      email: this.Email,
      pass: this.PasswordHash,
      phoneNum: this.PhoneNumber,
      createdAt: this.CreatedAt,
    });
  }
  public async ToRuntimeTypeAsync(): Promise<User> {
    return new User({
      email: this.Email,
      pass: this.PasswordHash,
      phoneNum: this.PhoneNumber,
      createdAt: this.CreatedAt,
    });
  }
  public static ParseSync(val: any): UserEntity {
    const { CreatedAt, Email, PasswordHash, PhoneNumber } =
      this._schema.parse(val);
    const tempObj = new UserEntity();
    tempObj.Email = Email;
    tempObj.PasswordHash = PasswordHash;
    tempObj.PhoneNumber = PhoneNumber;
    tempObj.CreatedAt = CreatedAt;
    return tempObj;
  }
  public static async ParseAsync(val: any): Promise<UserEntity> {
    return UserEntity.ParseSync(val);
  }
  public static TryParseSync(val: any): UserEntity | undefined {
    try {
      return UserEntity.ParseSync(val);
    } catch (e) {
      return undefined;
    }
  }
  public static async TryParseAsync(val: any): Promise<UserEntity | undefined> {
    return this.TryParseSync(val);
  }
}
