import { z } from "zod";
import Constants from "../../Constants";

export const UserSchema = z.object({
  Email: z
    .string()
    .email()
    .refine((x) => !!x, Constants.ExceptionMessages.emailEmpty)
    .transform((x) => x.toLocaleLowerCase()),
  Username: z
    .string()
    .refine(
      (x) => !!x && !x.includes(" "),
      Constants.ExceptionMessages.invalidOrEmptyUsername
    )
    .transform((x) => x.toLocaleLowerCase()),
  Name: z.string().nullable().optional(),
  Description: z.string().nullable().optional(),
  Verified: z.boolean().default(false),
  PasswordHash: z
    .string()
    .refine(
      (x) => !!x && !x.includes(" "),
      Constants.ExceptionMessages.passwordEmptyOrInvalid
    ),
  PhoneNumber: z
    .string()
    .optional()
    .nullable()
    .refine(
      (x) => !x || /^(\+44|0)\d{9,10}$/.test(x),
      Constants.ExceptionMessages.inncorrectPhoneFormat
    ),
  CreatedAt: z.coerce.date().default(new Date()),
  RoleName: z
    .string()
    .refine((x) => !!x && !x.includes(" "))
    .default(Constants.UserRoleNames.standardUser),
});

export type UserType = z.infer<typeof UserSchema>;
