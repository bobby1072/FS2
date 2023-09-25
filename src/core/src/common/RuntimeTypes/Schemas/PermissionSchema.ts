import { z } from "zod";

export const PermissionSchema = z.object({
  Buzzword: z.string().refine((x) => !!x),
});

export type PermissionType = z.infer<typeof PermissionSchema>;
