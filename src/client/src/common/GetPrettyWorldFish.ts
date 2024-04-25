import { IWorldFishModel } from "../models/IWorldFishModel";

export const getPrettyWorldFish = (fish: IWorldFishModel): string =>
  `${fish.englishName}${fish.nickname ? ` (${fish.nickname})` : ""}`;
