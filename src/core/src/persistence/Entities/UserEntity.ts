import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn } from "typeorm";
import { UserDBSchema, UserDbType } from "../Schemas/UserSchema";
import User from "../../common/RuntimeTypes/User";
import { BaseEntity } from "./BaseEntity";
import UserRoleEntity from "./UserRoleEntity";

@Entity({ name: "user" })
export default class UserEntity extends BaseEntity implements UserDbType {
  private static readonly _schema = UserDBSchema;
  @PrimaryColumn({ type: "text" })
  Username!: string;

  @Column({ type: "text", nullable: true })
  Name?: string | null;

  @Column({ type: "text", nullable: true })
  Description?: string | null;

  @Column({ type: "boolean", default: false })
  Verified!: boolean;

  @Column({ type: "text" })
  Email!: string;

  @Column({ type: "text", nullable: true })
  PhoneNumber?: string | null;

  @Column({ type: "text" })
  PasswordHash!: string;

  @Column({ type: "timestamptz", default: () => "CURRENT_TIMESTAMP" })
  CreatedAt!: Date;

  @Column({ type: "text" })
  RoleName!: string;

  @OneToOne(() => UserRoleEntity)
  @JoinColumn({ foreignKeyConstraintName: "fk_role_name" })
  Role?: UserRoleEntity | null;

  public ToRuntimeTypeSync(): User {
    const tempUserObj = this.ToJson();
    tempUserObj.Role = this.Role?.ToRuntimeTypeSync();
    return new User(tempUserObj);
  }
  public async ToRuntimeTypeAsync(): Promise<User> {
    const tempUserObj = this.ToJson();
    tempUserObj.Role = await this.Role?.ToRuntimeTypeAsync();
    return new User(tempUserObj);
  }
  public static ParseSync(val: any): UserEntity {
    const {
      CreatedAt,
      Email,
      PasswordHash,
      PhoneNumber,
      RoleName,
      Username,
      Verified,
      Description,
      Name,
    } = this._schema.parse(val);
    const tempObj = new UserEntity();
    tempObj.Email = Email;
    tempObj.RoleName = RoleName;
    tempObj.Username = Username;
    tempObj.Verified = Verified;
    tempObj.Description = Description;
    tempObj.Name = Name;
    tempObj.PasswordHash = PasswordHash;
    tempObj.PhoneNumber = PhoneNumber;
    tempObj.CreatedAt = CreatedAt;
    return tempObj;
  }
  public static async ParseAsync(val: any): Promise<UserEntity> {
    return UserEntity.ParseSync(val);
  }
}
