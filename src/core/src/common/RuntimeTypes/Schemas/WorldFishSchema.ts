import { z } from "zod";

export const WorldFishGenericSchema = z.object({
  ScientificName: z.string().optional().nullable(),
  Taxocode: z.string().default(""),
  A3Code: z.string().optional().nullable(),
  Isscaap: z.number().int().optional().nullable(),
  EnglishName: z.string().optional().nullable(),
  Nickname: z.string().optional().nullable(),
});
export type WorldFishGenericSchemaType = z.infer<typeof WorldFishGenericSchema>;
