import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid,
  Typography,
} from "@mui/material";
import { StyledDialogTitle } from "./StyledDialogTitle";
import { FieldErrors } from "react-hook-form";
import { ApiException } from "./ApiException";
import { ErrorComponent } from "./ErrorComponent";

export const YesOrNoModal: React.FC<{
  yesAction: () => void;
  closeModal: () => void;
  saveDisabled?: boolean;
  question: string | React.ReactNode;
  noAction?: () => void;
  title?: string;
  allErrors?: ApiException | FieldErrors;
}> = ({
  closeModal,
  allErrors,
  saveDisabled = false,
  question: message,
  yesAction,
  noAction,
  title,
}) => {
  return (
    <Dialog open onClose={closeModal}>
      <StyledDialogTitle>
        {title ? <Typography variant="h6">{title}</Typography> : null}
      </StyledDialogTitle>
      <DialogContent dividers>
        <Grid
          container
          spacing={2}
          padding={2}
          width={"100%"}
          justifyContent="center"
          alignItems="center"
        >
          <Grid item width="100%">
            <Typography variant="body1" textAlign="center">
              {message}
            </Typography>
          </Grid>
          {allErrors && (
            <Grid item width="100%">
              <ErrorComponent error={allErrors} />
            </Grid>
          )}
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
            <Button
              variant="outlined"
              onClick={noAction ? noAction : closeModal}
            >
              No
            </Button>
          </Grid>
          <Grid
            item
            width="40%"
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
            <Button
              variant="contained"
              disabled={saveDisabled}
              onClick={() => {
                yesAction();
              }}
            >
              Yes
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );
};
