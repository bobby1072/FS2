import { TokenDataSchema, TokenDataType } from "./Schemas/TokenDataSchema";

export default class TokenData implements TokenDataType {
  public readonly user: string;
  public readonly iat: number;
  private static readonly _schema = TokenDataSchema;
  public readonly exp: number;
  constructor(user: string, iat: number, exp: number) {
    const {
      exp: safeExp,
      iat: safeIat,
      user: safeUser,
    } = TokenData._schema.parse({ user: user, iat: iat, exp: exp });
    this.exp = safeExp;
    this.iat = safeIat;
    this.user = safeUser;
    return this;
  }
}
