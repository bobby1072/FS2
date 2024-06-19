import { Marker, Popup } from "react-leaflet";
import { RuntimePartialGroupCatchModel } from "../../models/IGroupCatchModel";
import { Button, Grid, Typography } from "@mui/material";
import { getPrettyWorldFishName } from "../../common/GetPrettyWorldFish";
import { Icon } from "leaflet";
import markerPhoto from "../MapComponents/images/map-marker.png";
import DeleteIcon from "@mui/icons-material/Delete";
import { prettyDateWithTime } from "../../utils/DateTime";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../../common/contexts/AbilitiesContext";
import { useCurrentUser } from "../../common/contexts/UserContext";
import { useDeleteCatchMutation } from "./hooks/DeleteCatch";
import { useEffect, useState } from "react";
import { useSnackbar } from "notistack";
import { YesOrNoModal } from "../../common/YesOrNoModal";
import { useNavigate } from "react-router-dom";
const GenericIconMarker = new Icon({
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
    error: deleteError,
    data: deletedId,
  } = useDeleteCatchMutation();
  const [openModal, setOpenModal] = useState<boolean>(false);
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (useSnackBarOnSuccess && deletedId)
      enqueueSnackbar("Catch deleted", { variant: "error" });
  }, [deletedId, enqueueSnackbar, useSnackBarOnSuccess]);
  return (
    <>
      <Button
        variant="contained"
        color="error"
        disabled={deleting}
        onClick={() => {
          setOpenModal(true);
        }}
      >
        <DeleteIcon />
      </Button>
      {openModal && (
        <YesOrNoModal
          closeModal={() => setOpenModal(false)}
          question={"Are you sure you want to delete this catch?"}
          yesAction={() => {
            deleteCatch(catchId);
          }}
          allErrors={deleteError ?? undefined}
          saveDisabled={deleting}
        />
      )}
    </>
  );
};

export const CatchMarkerForPartialCatch: React.FC<{
  groupCatch: RuntimePartialGroupCatchModel;
  useSnackBarOnSuccess?: boolean;
  groupId: string;
}> = ({
  useSnackBarOnSuccess = false,
  groupId,
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
  const navigate = useNavigate();
  return (
    <CatchMarker position={[latitude, longitude]}>
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
            <Typography variant="h5">
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
            <Button
              variant="outlined"
              onClick={(e) => {
                e.preventDefault();
                navigate(`/GroupCatch/${catchId}`);
              }}
              href={`/GroupCatch/${catchId}`}
            >
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
              <Grid
                container
                justifyContent="center"
                alignItems="center"
                spacing={0.8}
              >
                <Grid item width="100%">
                  <DeleteButtonForMarker
                    catchId={catchId}
                    useSnackBarOnSuccess={useSnackBarOnSuccess}
                  />
                </Grid>
              </Grid>
            </Grid>
          )}
        </Grid>
      </Popup>
    </CatchMarker>
  );
};

export const SimpleLongLatCatchMarkerWithPopup: React.FC<{
  position: [number, number];
}> = ({ position: [lat, lng] }) => {
  return (
    <CatchMarker position={[lat, lng]}>
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
            <Typography variant="subtitle2">Latitude: {lat}</Typography>
          </Grid>
          <Grid item>
            <Typography variant="subtitle2">Longitude: {lng}</Typography>
          </Grid>
        </Grid>
      </Popup>
    </CatchMarker>
  );
};

export const CatchMarker: React.FC<{
  children?: React.ReactNode;
  position: [number, number];
}> = ({ position: [latitude, longitude], children }) => {
  return (
    <Marker position={[latitude, longitude]} icon={GenericIconMarker}>
      {children}
    </Marker>
  );
};
