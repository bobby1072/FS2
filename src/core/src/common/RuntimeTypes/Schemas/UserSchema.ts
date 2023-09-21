import { z } from "zod";
import Constants from "../../Constants";

export const UserSchema = z.object({
  Email: z
    .string()
    .email()
    .refine((x) => !!x, Constants.ExceptionMessages.emailEmpty)
    .transform((x) => x.toLowerCase()),
  Username: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.invalidOrEmptyUsername),
  Name: z.string().nullable().optional(),
  Description: z.string().nullable().optional(),
  Verified: z.boolean().default(false),
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
  CreatedAt: z.coerce.date().default(new Date()),
  RoleName: z.string().refine((x) => !!x),
});

export type UserType = z.infer<typeof UserSchema>;
