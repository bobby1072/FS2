import { z } from "zod";
import Constants from "../../Constants";

export const TokenDataSchema = z.object({
  user: z.string().refine((x) => !!x, Constants.ExceptionMessages.emailEmpty),
  iat: z
    .number()
    .int()
    .refine(
      (x) => x < new Date().getTime(),
      Constants.ExceptionMessages.tokenExpired
    ),
  exp: z
    .number()
    .int()
    .refine(
      (x) => x > new Date().getTime(),
      Constants.ExceptionMessages.invalidToken
    ),
});

export type TokenDataType = z.infer<typeof TokenDataSchema>;
