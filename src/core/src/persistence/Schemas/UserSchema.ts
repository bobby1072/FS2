import { z } from "zod";
import Constants from "../../common/Constants";

export const UserDBSchema = z.object({
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

export type UserType = z.infer<typeof UserDBSchema>;
