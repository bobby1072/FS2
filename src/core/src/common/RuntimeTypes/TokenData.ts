import { sign, verify } from "jsonwebtoken";
import ApiError from "../ApiError";
import Constants from "../Constants";
import { TokenDataSchema, TokenDataType } from "./Schemas/TokenDataSchema";

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
  public static DecodeToken(token: string): TokenData {
    try {
      if (token.includes("Bearer ")) token = token.replace("Bearer ", "");
      const decodedToken = verify(
        token,
        process.env.SK ?? "dev_secret_key"
      ) as any;
      return new TokenData({
        user: decodedToken.user,
        iat: decodedToken.iat,
        exp: decodedToken.exp,
        roleName: decodedToken.roleName,
      });
    } catch (e) {
      throw new ApiError(Constants.ExceptionMessages.invalidToken, 401);
    }
  }
  public static async DecodeTokenAsync(token: string): Promise<TokenData> {
    return this.DecodeToken(token);
  }
  public static EncodeToken(user: string, roleName: string): string {
    return sign(
      {
        user,
        roleName,
      },
      process.env.SK ?? "dev_secret_key",
      { algorithm: "HS256", expiresIn: "1h" }
    );
  }
}
