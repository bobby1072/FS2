import { z } from "zod";

export const UserRoleDbSchema = z.object({
  RoleName: z.string().refine((x) => !!x),
  UserPermissions: z.array(z.string().refine((x) => !!x)),
  GroupPermissions: z.array(z.string().refine((x) => !!x)),
});

export type UserRoleDBType = z.infer<typeof UserRoleDbSchema>;
