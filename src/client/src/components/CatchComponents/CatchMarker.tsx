import { Marker, Popup } from "react-leaflet";
import { IPartialGroupCatchModel } from "../../models/IGroupCatchModel";
import { Button, Grid, Typography } from "@mui/material";
import { getPrettyWorldFish } from "../../common/GetPrettyWorldFish";
import { Icon } from "leaflet";
import markerPhoto from "../../data/images/map-marker.png";
import { prettyDateWithTime } from "../../utils/DateTime";
const iconMarker = new Icon({
  iconUrl: markerPhoto,
  iconSize: [34, 34],
});
export const CatchMarker: React.FC<{
  groupCatch: IPartialGroupCatchModel;
}> = ({
  groupCatch: {
    latitude,
    longitude,
    worldFish,
    species,
    user: { username },
    id,
    caughtAt,
    weight,
  },
}) => {
  return (
    <Marker position={[latitude, longitude]} icon={iconMarker}>
      <Popup>
        <Grid
          container
          direction="column"
          justifyContent="center"
          alignItems="center"
          padding={0.5}
          spacing={0.5}
          textAlign="center"
        >
          <Grid item>
            <Typography variant="h6">
              <strong>
                {worldFish ? getPrettyWorldFish(worldFish) : species}
              </strong>
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="subtitle2">
              <strong>Caught by:</strong> {username}
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="subtitle2">
              <strong>Caught at:</strong>{" "}
              {prettyDateWithTime(new Date(caughtAt))}
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="subtitle2">
              <strong>Weighing:</strong> {weight}
            </Typography>
          </Grid>
          <Grid item width="90%">
            <Button variant="outlined" href={`/GroupCatch/${id}`}>
              Go to full catch
            </Button>
          </Grid>
        </Grid>
      </Popup>
    </Marker>
  );
};
