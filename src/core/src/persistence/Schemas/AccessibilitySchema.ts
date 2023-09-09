import { z } from "zod";

export const AccessibilityDBSchema = z.object({
  Title: z.string().refine((x) => !!x),
});

export type AccessibilityDBType = z.infer<typeof AccessibilityDBSchema>;
