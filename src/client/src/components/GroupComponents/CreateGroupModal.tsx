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
import { GroupModel } from "../../models/GroupModel";
import { useState } from "react";

export const CreateGroupModal: React.FC<{
  closeModal: () => void;
  group?: GroupModel;
}> = ({ closeModal, group }) => {
  const [saveDisbaled, setSaveDisabled] = useState<boolean>(true);
  return (
    <Dialog open onClose={closeModal} fullWidth maxWidth="sm" scroll="paper">
      <StyledDialogTitle>
        <Typography variant="h6">Discard Warning</Typography>
      </StyledDialogTitle>
      <DialogContent dividers={true}>
        <CreateGroupModalForm
          {...group}
          setIsDirty={(boolVal: boolean) => setSaveDisabled(boolVal)}
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
          <Grid item width="25%">
            <Button variant="outlined" onClick={closeModal}>
              Cancel
            </Button>
          </Grid>
          <Grid item width="25%">
            <Button
              variant="contained"
              type="submit"
              aria-label="Submit"
              disabled={saveDisbaled}
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
