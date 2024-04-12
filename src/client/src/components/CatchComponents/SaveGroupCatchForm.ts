import { z } from "zod";

const formSchema = z.object({
  id: z.string().optional().nullable(),
  groupId: z.string(),
  species: z.string(),
  worldFishTaxocode: z.string().optional().nullable(),
  weight: z.number(),
  length: z.number(),
  description: z.string().optional().nullable(),
  latitude: z.number(),
  longitude: z.number(),
  caughtAt: z.date(),
  catchPhoto: z.string().optional().nullable(),
});

export type SaveCatchInput = z.infer<typeof formSchema>;
