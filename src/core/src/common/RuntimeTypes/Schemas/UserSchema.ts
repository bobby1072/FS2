import { z } from "zod";
import Constants from "../../Constants";

export const UserSchema = z.object({
  Email: z
    .string()
    .email()
    .refine((x) => !!x, Constants.ExceptionMessages.emailEmpty),
  PasswordHash: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.passwordEmpty),
  PhoneNumber: z
    .string()
    .optional()
    .nullable()
    .refine(
      (x) => !x || /^(\+44|0)\d{9,10}$/.test(x),
      Constants.ExceptionMessages.inncorrectPhoneFormat
    ),
  CreatedAt: z.date(),
});

export type RunUserType = z.infer<typeof UserSchema>;
