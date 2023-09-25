import { z } from "zod";
import Constants from "../../Constants";

export const TokenDataSchema = z.object({
  user: z.string().refine((x) => !!x, Constants.ExceptionMessages.invalidToken),
  iat: z.number().int(),
  exp: z.number().int(),
  roleName: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.invalidToken),
});

export type TokenDataType = z.infer<typeof TokenDataSchema>;
