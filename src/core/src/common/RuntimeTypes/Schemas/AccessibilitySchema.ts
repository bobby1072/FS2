import { z } from "zod";

export const AccessibilitySchema = z.object({
  Title: z.string().refine((x) => !!x),
});

export type AccessibilityType = z.infer<typeof AccessibilitySchema>;
