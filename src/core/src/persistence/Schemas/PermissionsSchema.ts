import { z } from "zod";

export const PermissionsDBSchema = z.object({
  Buzzword: z.string().refine((x) => !!x),
});

export type PermissionsDBType = z.infer<typeof PermissionsDBSchema>;
