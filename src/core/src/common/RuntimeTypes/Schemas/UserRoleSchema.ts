import { z } from "zod";

export const UserRoleSchema = z.object({
  RoleName: z.string().refine((x) => !!x),
  GroupPermissions: z.array(z.string().refine((x) => !!x)),
});

export type UserRoleType = z.infer<typeof UserRoleSchema>;
