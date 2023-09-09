import { z } from "zod";

export const PermissionDBSchema = z.object({
  Buzzword: z.string().refine((x) => !!x),
});

export type PermissionDBType = z.infer<typeof PermissionDBSchema>;
