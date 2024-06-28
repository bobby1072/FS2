import {
  IPartialGroupCatchModel,
  RuntimePartialGroupCatchModel,
} from "../../src/models/IGroupCatchModel";
import { faker } from "@faker-js/faker";
export default abstract class GroupCatchMockDataProvider {
  public static BuildRuntimePartial(
    opts?: Partial<RuntimePartialGroupCatchModel>
  ): RuntimePartialGroupCatchModel {
    return new RuntimePartialGroupCatchModel({
      caughtAt:
        opts?.caughtAt?.toISOString() ?? faker.date.past().toISOString(),
      id: opts?.id ?? faker.string.uuid(),
      groupId: opts?.groupId ?? faker.string.uuid(),
      latitude:
        opts?.latitude ??
        faker.number.int({
          min: -90,
          max: 90,
        }),
      longitude:
        opts?.longitude ??
        faker.number.int({
          max: 180,
          min: -180,
        }),
      species: opts?.species ?? faker.animal.fish(),
      weight: opts?.weight ?? faker.number.int({ min: 1, max: 100 }),
      worldFish: opts?.worldFish ?? {
        taxocode: faker.number.bigInt().toString(),
        englishName: faker.animal.fish(),
      },
      user: {
        emailVerified: opts?.user?.emailVerified ?? faker.datatype.boolean(),
        id: opts?.user?.id ?? faker.string.uuid(),
        username: opts?.user?.username ?? faker.internet.userName(),
      },
      length: opts?.length ?? faker.number.int({ min: 1, max: 100 }),
    });
  }
}
