import { CatchMarkerForPartialCatch } from "./CatchMarker";
import { render } from "../../../test/utils/test-utils";
import { faker } from "@faker-js/faker";
import GroupCatchMockDataProvider from "../../../test/mocks/GroupCatch";
import { MapContainer } from "react-leaflet";
describe(CatchMarkerForPartialCatch.name, () => {
  it("Should render correct details", async () => {
    const fakeGroupId = faker.string.uuid();
    const fakePartialCatch = GroupCatchMockDataProvider.BuildRuntimePartial({
      groupId: fakeGroupId,
    });
    render(
      <CatchMarkerForPartialCatch
        groupId={fakeGroupId}
        groupCatch={fakePartialCatch}
      />,
      { wrapper: MapContainer }
    );
  });
});
