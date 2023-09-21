import { z } from "zod";

export const UsernamePassword = z.object({
  Username: z.string().default(""),
  Password: z.string().default(""),
});
export type UsernamePasswordType = z.infer<typeof UsernamePassword>;
