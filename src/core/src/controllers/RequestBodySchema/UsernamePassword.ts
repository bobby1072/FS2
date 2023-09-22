import { z } from "zod";
import Constants from "../../common/Constants";

export const UsernamePassword = z.object({
  Username: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.invalidOrEmptyUsername)
    .transform((x) => x.toLocaleLowerCase()),
  Password: z
    .string()
    .refine((x) => !!x, Constants.ExceptionMessages.passwordEmptyOrInvalid),
});
export type UsernamePasswordType = z.infer<typeof UsernamePassword>;
