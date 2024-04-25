import { Marker, Popup } from "react-leaflet";
import { IPartialGroupCatchModel } from "../../models/IGroupCatchModel";
import { Grid, Typography } from "@mui/material";
import { getPrettyWorldFish } from "../../common/GetPrettyWorldFish";
import RoomIcon from "@mui/icons-material/Room";

export const CatchMarker: React.FC<{
  groupCatch: IPartialGroupCatchModel;
}> = ({ groupCatch: { latitude, longitude, worldFish, species } }) => {
  return (
    <Marker position={[latitude, longitude]}>
      <Popup>
        <Grid container direction="column" spacing={1} padding={1}>
          <Grid item>
            <Typography variant="h6">
              {worldFish ? getPrettyWorldFish(worldFish) : species}
            </Typography>
          </Grid>
        </Grid>
      </Popup>
    </Marker>
  );
};
