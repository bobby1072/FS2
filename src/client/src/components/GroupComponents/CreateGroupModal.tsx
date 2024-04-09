import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid,
  Typography,
} from "@mui/material";
import { StyledDialogTitle } from "../../common/StyledDialogTitle";
import { CreateGroupModalForm } from "./CreateGroupModalForm";
import { IGroupModel } from "../../models/IGroupModel";
import { useEffect, useState } from "react";
import { useDeleteGroupMutation } from "./hooks/DeleteGroupMutation";
import { useSnackbar } from "notistack";

export const CreateGroupModal: React.FC<{
  closeModal: () => void;
  group?: IGroupModel;
}> = ({ closeModal, group }) => {
  const [saveDisabled, setSaveDisabled] = useState<boolean>(true);
  const { enqueueSnackbar } = useSnackbar();
  const {
    data: deletedGroupId,
    mutate: deleteGroup,
    isLoading,
  } = useDeleteGroupMutation();
  useEffect(() => {
    if (deletedGroupId) {
      enqueueSnackbar("Group deleted", { variant: "error" });
      closeModal();
    }
  }, [deletedGroupId, closeModal, enqueueSnackbar]);
  return (
    <Dialog open onClose={closeModal} fullWidth maxWidth="sm" scroll="paper">
      <StyledDialogTitle>
        <Typography variant="h6">
          {group ? "Edit group" : "Create group"}
        </Typography>
      </StyledDialogTitle>
      <DialogContent dividers={true}>
        <CreateGroupModalForm
          group={group}
          closeModal={closeModal}
          useSnackBarOnSuccess={true}
          setIsSaveDisabled={(boolVal: boolean) => setSaveDisabled(boolVal)}
        />
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
            <Grid container direction="row" spacing={1}>
              <Grid item>
                <Button variant="outlined" onClick={closeModal}>
                  Cancel
                </Button>
              </Grid>
              {group && (
                <Grid item>
                  <Button
                    variant="contained"
                    color="error"
                    onClick={() => deleteGroup({ groupId: group.id! })}
                    disabled={isLoading}
                  >
                    Delete
                  </Button>
                </Grid>
              )}
            </Grid>
          </Grid>
          <Grid
            item
            width="40%"
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
            <Button
              variant="contained"
              type="submit"
              aria-label="Submit"
              disabled={saveDisabled}
              form="groupSaveForm"
            >
              Save
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );
};
