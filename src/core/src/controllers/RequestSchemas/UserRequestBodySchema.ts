import { z } from "zod";
import Constants from "../../common/Constants";

export const UserApiRequestBodySchema = z.object({
  email: z
    .string()
    .email()
    .refine((x) => !!x, Constants.ExceptionMessages.emailEmpty),
  password: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.passwordEmpty),
  phoneNumber: z
    .string()
    .optional()
    .nullable()
    .refine(
      (x) => !x || /^(\+44|0)\d{9,10}$/.test(x),
      Constants.ExceptionMessages.inncorrectPhoneFormat
    ),
});

export type UserRequestBodySchema = z.infer<typeof UserApiRequestBodySchema>;
