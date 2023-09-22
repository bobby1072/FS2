import { z } from "zod";

export const WorldFishGenericDbSchema = z.object({
  ScientificName: z.string().optional().nullable(),
  Taxocode: z.string().default(""),
  A3Code: z.string().optional().nullable(),
  Isscaap: z.number().int().optional().nullable(),
  EnglishName: z.string().optional().nullable(),
  Nickname: z.string().optional().nullable(),
});
export type WorldFishGenericDbSchemaType = z.infer<
  typeof WorldFishGenericDbSchema
>;
