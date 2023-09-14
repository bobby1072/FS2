import { z } from "zod";
import Constants from "../../common/Constants";
import { UserRoleDbSchema } from "./UserRoleSchema";

export const UserDBSchema = z.object({
  Username: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.invalidOrEmptyUsername),
  Name: z.string().nullable().optional(),
  Description: z.string().nullable().optional(),
  Verified: z.boolean().default(false),
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
  RoleName: z.string().refine((x) => !!x),
});

export type UserDbType = z.infer<typeof UserDBSchema>;
