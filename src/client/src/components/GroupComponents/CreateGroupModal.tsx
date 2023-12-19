import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Typography,
} from "@mui/material";
import { StyledDialogTitle } from "../../common/StyledDialogTitle";

export const CreateGroupModal: React.FC<{ closeModal: () => void }> = ({
  closeModal,
}) => {
  return (
    <Dialog open onClose={closeModal} fullWidth maxWidth="sm" scroll="paper">
      <StyledDialogTitle>
        <Typography variant="h6">Discard Warning</Typography>
      </StyledDialogTitle>
      <DialogContent dividers={true}></DialogContent>
      <DialogActions>
        <Button variant="outlined">Cancel</Button>
        <Button variant="contained" type="submit">
          Save
        </Button>
      </DialogActions>
    </Dialog>
  );
};
