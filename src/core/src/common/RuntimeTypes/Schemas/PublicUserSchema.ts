import { z } from "zod";
import Constants from "../../Constants";

export const PublicUserSchema = z.object({
  Username: z
    .string()
    .refine(
      (x) => !!x && !x.includes(" "),
      Constants.ExceptionMessages.invalidOrEmptyUsername
    )
    .transform((x) => x.toLocaleLowerCase()),
  Name: z.string().nullable().optional(),
  Description: z.string().nullable().optional(),
  CreatedAt: z.coerce.date().default(new Date()),
});

export type PublicUserType = z.infer<typeof PublicUserSchema>;
