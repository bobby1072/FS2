import {
  Accordion,
  AccordionDetails,
  Box,
  Grid,
  IconButton,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { Close } from "@mui/icons-material";

import { RuntimeGroupCatchModel } from "../../models/IGroupCatchModel";
import {
  formatHowLongAgoString,
  prettyDateWithTime,
} from "../../utils/DateTime";
import { getPrettyWorldFishName } from "../../common/GetPrettyWorldFish";
import EditIcon from "@mui/icons-material/Edit";

import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../../common/contexts/AbilitiesContext";
import { useCurrentUser } from "../../common/contexts/UserContext";
import { useState } from "react";
import { SaveGroupCatchForm } from "./SaveGroupCatchForm";

export const CatchPaperForm: React.FC<{
  fullCatch: RuntimeGroupCatchModel;
}> = ({ fullCatch }) => {
  const [editMode, setEditMode] = useState<boolean>(false);
  const { permissionManager } = useCurrentPermissionManager();
  const { id: currentUserId } = useCurrentUser();
  const canEdit =
    currentUserId === fullCatch.userId ||
    permissionManager.Can(
      PermissionActions.Manage,
      fullCatch.groupId,
      PermissionFields.GroupCatch
    );
  return editMode ? (
    <Accordion expanded>
      <AccordionDetails>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          direction={"column"}
          spacing={0.2}
        >
          <Grid
            item
            width="100%"
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
            <IconButton
              color="default"
              size="small"
              onClick={() => setEditMode(false)}
            >
              <Close />
            </IconButton>
          </Grid>
          <Grid item width="100%">
            <SaveGroupCatchForm
              useSnackBarOnSuccess
              groupCatch={fullCatch.Serialise()}
              closeForm={() => setEditMode(false)}
            />
          </Grid>
        </Grid>
      </AccordionDetails>
    </Accordion>
  ) : (
    <Paper elevation={2}>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        spacing={2}
        padding={2}
      >
        {canEdit && (
          <Grid
            item
            width={"100%"}
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
            <IconButton
              color="primary"
              size="small"
              onClick={() => setEditMode(true)}
            >
              <EditIcon />
            </IconButton>
          </Grid>
        )}
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
            Caught by <strong>{fullCatch.user!.username}</strong> on{" "}
            <strong>{prettyDateWithTime(fullCatch.caughtAt)}</strong>
          </Typography>
        </Grid>
        <Grid item width={"100%"}>
          <Typography
            variant="body2"
            textAlign="center"
            fontSize={14}
            overflow="auto"
          >
            Posted{" "}
            <strong>{formatHowLongAgoString(fullCatch.createdAt)}</strong>
          </Typography>
        </Grid>
        {fullCatch.catchPhoto && (
          <Grid item>
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
          </Grid>
        )}
        {fullCatch.description && (
          <Grid item width={"100%"}>
            <TextField
              fullWidth
              disabled
              multiline
              value={fullCatch.description}
              variant="outlined"
              label="Description"
            />
          </Grid>
        )}
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
  );
};
