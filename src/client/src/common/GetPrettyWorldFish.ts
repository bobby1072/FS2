import { IWorldFishModel } from "../models/IWorldFishModel";

export const getPrettyWorldFishName = (fish: IWorldFishModel): string =>
  `${fish.englishName}${fish.nickname ? ` (${fish.nickname})` : ""}`;
