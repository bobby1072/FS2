import { TokenDataSchema, TokenDataType } from "./Schemas/TokenDataSchema";
import User from "./User";

export default class TokenData implements TokenDataType {
  public readonly user: string;
  public readonly roleName: string;
  public readonly iat: number;
  private static readonly _schema = TokenDataSchema;
  public readonly exp: number;
  constructor({
    exp,
    iat,
    roleName,
    user,
  }: {
    user: string;
    iat: number;
    exp: number;
    roleName: string;
  }) {
    const {
      exp: safeExp,
      iat: safeIat,
      user: safeUser,
      roleName: safeRoleName,
    } = TokenData._schema.parse({ user, iat, exp, roleName });
    this.exp = safeExp;
    this.roleName = safeRoleName;
    this.iat = safeIat;
    this.user = safeUser;
    return this;
  }
  public Encode() {
    return User.EncodeToken(this.user, this.roleName);
  }
}
