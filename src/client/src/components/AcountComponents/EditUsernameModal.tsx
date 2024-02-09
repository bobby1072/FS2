import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid,
  TextField,
  Typography,
} from "@mui/material";
import { StyledDialogTitle } from "../../common/StyledDialogTitle";
import { useEffect, useState } from "react";
import { useSaveUsernameMutation } from "./hooks/SaveUsername";
import { useSnackbar } from "notistack";

export const EditUsernameModal: React.FC<{ closeModal: () => void }> = ({
  closeModal,
}) => {
  const [newUsername, setNewUsername] = useState<string>();
  const { data, isLoading, error, mutate } = useSaveUsernameMutation();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (data) {
      enqueueSnackbar("Username updated", { variant: "success" });
      closeModal();
    }
  }, [data, enqueueSnackbar, closeModal]);
  return (
    <Dialog open fullWidth maxWidth="sm" scroll="paper" onClose={closeModal}>
      <StyledDialogTitle>
        <Typography variant="h6">Edit username</Typography>
      </StyledDialogTitle>
      <DialogContent dividers={true}>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          width="100%"
          direction="column"
          spacing={1}
          padding={1}
        >
          <Grid item>
            <TextField
              onChange={(e) => {
                setNewUsername(e.target.value);
              }}
              label="New username"
              fullWidth
            />
          </Grid>
          {error && <Alert severity="error">{error.message}</Alert>}
        </Grid>
      </DialogContent>
      <DialogActions>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          width="100%"
          direction="row"
          spacing={1}
          padding={1}
        >
          <Grid
            item
            width="40%"
            sx={{ display: "flex", justifyContent: "flex-start" }}
          >
            <Button variant="outlined" onClick={closeModal}>
              Cancel
            </Button>
          </Grid>
          <Grid
            item
            width="40%"
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
            <Button
              variant="contained"
              disabled={isLoading || !newUsername}
              onClick={() => {
                if (newUsername) {
                  mutate({ newUsername: newUsername });
                }
              }}
            >
              Save
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );
};
