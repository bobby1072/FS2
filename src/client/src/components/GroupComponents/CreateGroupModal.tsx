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

export const CreateGroupModal: React.FC<{
  closeModal: () => void;
  group?: GroupModel;
}> = ({ closeModal }) => {
  return (
    <Dialog open onClose={closeModal} fullWidth maxWidth="sm" scroll="paper">
      <StyledDialogTitle>
        <Typography variant="h6">Discard Warning</Typography>
      </StyledDialogTitle>
      <DialogContent dividers={true}>
        <CreateGroupModalForm />
      </DialogContent>
      <DialogActions>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          spacing={2}
          width="100%"
          direction="row"
          padding={1}
        >
          <Grid item width={"50%"}>
            <Button variant="outlined">Cancel</Button>
          </Grid>
          <Grid item width={"50%"}>
            <Button variant="contained" type="submit">
              Save
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );
};
