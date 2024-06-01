import { useParams } from "react-router-dom";
import { useGetFullCatchQuery } from "../components/CatchComponents/hooks/GetFullCatch";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../common/contexts/AbilitiesContext";
import { ErrorComponent } from "../common/ErrorComponent";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { Box, Grid, Paper, TextField, Typography } from "@mui/material";
import { getPrettyWorldFishName } from "../common/GetPrettyWorldFish";
import { formatHowLongAgoString, prettyDateWithTime } from "../utils/DateTime";
import { GenerateMap } from "../components/MapComponents/GenerateMap";
import { CatchMarker } from "../components/CatchComponents/CatchMarker";
import { Popup } from "react-leaflet";

export const IndividualCatchPage: React.FC = () => {
  const { id: catchId } = useParams<{ id: string }>();
  const {
    data: fullCatch,
    isLoading: catchLoading,
    error: catchError,
  } = useGetFullCatchQuery(catchId);
  const { permissionManager } = useCurrentPermissionManager();
  if (catchLoading) return <Loading fullScreen />;
  else if (
    fullCatch &&
    !fullCatch.group?.catchesPublic &&
    !permissionManager.Can(
      PermissionActions.Read,
      fullCatch.groupId,
      PermissionFields.GroupCatch
    )
  ) {
    return (
      <ErrorComponent
        fullScreen
        error={new Error("You do not have permissions to view this catch")}
      />
    );
  } else if (catchError)
    return <ErrorComponent fullScreen error={catchError} />;
  else if (!fullCatch) return <ErrorComponent fullScreen />;
  const catchPosition = fullCatch.GetPosition();
  return (
    <PageBase>
      <AppAndDraw>
        <Grid container justifyContent="center" alignItems="center" spacing={2}>
          <Grid item width={"80%"}>
            <Paper elevation={2}>
              <Grid
                container
                justifyContent="center"
                alignItems="center"
                spacing={2}
                padding={4}
              >
                <Grid item width={"100%"}>
                  <Typography
                    variant="h3"
                    textAlign="center"
                    fontSize={50}
                    overflow="auto"
                  >
                    {fullCatch.worldFish
                      ? getPrettyWorldFishName(fullCatch.worldFish)
                      : fullCatch.species}
                  </Typography>
                </Grid>
                <Grid item width={"100%"}>
                  <Typography
                    variant="body2"
                    textAlign="center"
                    fontSize={16}
                    overflow="auto"
                  >
                    Caught by <strong>{fullCatch.user!.username}</strong> on <strong>{prettyDateWithTime(fullCatch.caughtAt)}</strong>
                  </Typography>
                </Grid>
                <Grid item width={"100%"}>
                  <Typography
                    variant="body2"
                    textAlign="center"
                    fontSize={14}
                    overflow="auto"
                  >
                    Posted <strong>{formatHowLongAgoString(fullCatch.createdAt)}</strong>
                  </Typography>
                </Grid>
                {fullCatch.catchPhoto && <Grid item>
                  <Box
                    component="img"
                    sx={{
                      border: "0.1px solid #999999",
                      maxHeight: "80vh",
                      width: "100%",
                    }}
                    src={`data:image/jpeg;base64,${fullCatch.catchPhoto}`}
                    alt={`catch photo: ${fullCatch.id}`}
                  />
                </Grid>}
                {fullCatch.description && <Grid item width={"100%"}>
                  <TextField
                    fullWidth
                    disabled
                    multiline
                    value={fullCatch.description}
                    variant="outlined"
                    label="Description"
                  />
                </Grid>}
                <Grid item width="50%">
                  <TextField
                    disabled
                    fullWidth
                    variant="outlined"
                    label="Weight"
                    value={`${fullCatch.weight} lbs`}
                  />
                </Grid>
                <Grid item width="50%">
                  <TextField
                    disabled
                    fullWidth
                    variant="outlined"
                    label="Length"
                    value={`${fullCatch.length} cm`}
                  />
                </Grid>
              </Grid>
            </Paper>
          </Grid>
          <Grid item width={"100%"}>
            <GenerateMap
              center={catchPosition}
            >
              <CatchMarker position={catchPosition}>
                <Popup>
                  <Grid
                    container
                    direction="column"
                    justifyContent="center"
                    alignItems="center"
                    padding={0.5}
                    spacing={0.8}
                    textAlign="center"
                  >
                    <Grid item>
                      <Typography variant="subtitle2">
                        Latitude: {fullCatch.latitude}
                      </Typography>
                    </Grid>
                    <Grid item>
                      <Typography variant="subtitle2">
                        Longitude: {fullCatch.longitude}
                      </Typography>
                    </Grid>
                  </Grid>
                </Popup>
              </CatchMarker>
            </GenerateMap>
          </Grid>
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
