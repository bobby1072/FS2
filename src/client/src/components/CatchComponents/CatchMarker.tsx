import { Marker, Popup } from "react-leaflet";
import {
  IGroupCatchModel,
  IPartialGroupCatchModel,
} from "../../models/IGroupCatchModel";
import { Button, Grid, Typography } from "@mui/material";
import { getPrettyWorldFishName } from "../../common/GetPrettyWorldFish";
import { Icon } from "leaflet";
import markerPhoto from "../../data/images/map-marker.png";
import DeleteIcon from "@mui/icons-material/Delete";
import { prettyDateWithTime } from "../../utils/DateTime";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../../common/contexts/AbilitiesContext";
import { useCurrentUser } from "../../common/contexts/UserContext";
import { useDeleteCatchMutation } from "./hooks/DeleteCatch";
import { useEffect } from "react";
import { useSnackbar } from "notistack";
export const GenericIconMarker = new Icon({
  iconUrl: markerPhoto,
  iconSize: [35, 35],
});
const DeleteButtonForMarker: React.FC<{
  useSnackBarOnSuccess?: boolean;
  catchId: string;
}> = ({ useSnackBarOnSuccess = false, catchId }) => {
  const {
    mutate: deleteCatch,
    isLoading: deleting,
    data: deletedId,
  } = useDeleteCatchMutation();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (useSnackBarOnSuccess && deletedId) {
      enqueueSnackbar("Catch deleted", { variant: "error" });
    }
  }, [deletedId, enqueueSnackbar, useSnackBarOnSuccess]);
  return (
    <Button
      variant="contained"
      color="error"
      disabled={deleting}
      onClick={() => {
        deleteCatch(catchId);
      }}
    >
      <DeleteIcon />
    </Button>
  );
};
export const CatchMarker: React.FC<{
  groupCatch: IPartialGroupCatchModel;
  useSnackBarOnSuccess?: boolean;
  groupId: string;
  formUpdates?: {
    setCatchToEdit: (gc: IGroupCatchModel) => void;
  };
}> = ({
  useSnackBarOnSuccess = false,
  groupId,
  formUpdates,
  groupCatch: {
    latitude,
    longitude,
    worldFish,
    species,
    user: { id: catchUserId, username: catchUsername },
    id: catchId,
    caughtAt,
    weight,
  },
}) => {
  const { id: currentUserId } = useCurrentUser();
  const { permissionManager } = useCurrentPermissionManager();
  // const { mutate: getFullCatch, data: fullCatch } = useGetFullCatchMutation();
  // useEffect(() => {
  //   if (formUpdates && catchUserId === currentUserId) {
  //     getFullCatch(catchId);
  //   }
  // }, [formUpdates, getFullCatch, catchId, catchUserId, currentUserId]);
  // useEffect(() => {
  //   if (fullCatch) {
  //     formUpdates?.setCatchToEdit(fullCatch);
  //   }
  // }, [fullCatch, formUpdates]);
  return (
    <Marker position={[latitude, longitude]} icon={GenericIconMarker}>
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
            <Typography variant="h5" sx={{ textDecoration: "underline" }}>
              <strong>
                {worldFish ? getPrettyWorldFishName(worldFish) : species}
              </strong>
            </Typography>
          </Grid>
          <Grid item>
            <Typography variant="subtitle2">
              <strong>Caught by:</strong> {catchUsername}
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
            <Button variant="outlined" href={`/GroupCatch/${catchId}`}>
              Go to full catch
            </Button>
          </Grid>
          {(permissionManager.Can(
            PermissionActions.Manage,
            groupId,
            PermissionFields.GroupCatch
          ) ||
            currentUserId === catchUserId) && (
            <Grid item width="40%">
              <DeleteButtonForMarker
                catchId={catchId}
                useSnackBarOnSuccess={useSnackBarOnSuccess}
              />
            </Grid>
          )}
        </Grid>
      </Popup>
    </Marker>
  );
};
