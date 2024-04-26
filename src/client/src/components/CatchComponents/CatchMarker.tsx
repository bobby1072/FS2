import { Marker, Popup } from "react-leaflet";
import { IPartialGroupCatchModel } from "../../models/IGroupCatchModel";
import { Button, Grid, Typography } from "@mui/material";
import { getPrettyWorldFish } from "../../common/GetPrettyWorldFish";
import { Icon } from "leaflet";
import markerPhoto from "../../data/images/map-marker.png";
import { prettyDateWithTime } from "../../utils/DateTime";
import { useCurrentPermissionManager } from "../../common/contexts/AbilitiesContext";
import { useFormContext } from "react-hook-form";
import { SaveCatchInput } from "./SaveGroupCatchForm";
const iconMarker = new Icon({
  iconUrl: markerPhoto,
  iconSize: [35, 35],
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
  const { permissionManager } = useCurrentPermissionManager();
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
            <Typography variant="h5" sx={{ textDecoration: "underline" }}>
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
              <strong>Weighing:</strong> {weight} kg
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
