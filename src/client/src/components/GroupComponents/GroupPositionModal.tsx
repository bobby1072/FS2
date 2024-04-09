import React, { useEffect, useState } from "react";
import { IGroupPositionModel } from "../../models/IGroupPositionModel";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  FormControlLabel,
  Grid,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { StyledDialogTitle } from "../../common/StyledDialogTitle";
import { FieldErrors, useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSavePositionMutation } from "./hooks/SavePositionMutation";
import { ApiException } from "../../common/ApiException";
import { useSnackbar } from "notistack";

const formSchema = z.object({
  id: z.string().optional().nullable(),
  groupId: z.string(),
  name: z.string(),
  canManageGroup: z.boolean(),
  canReadCatches: z.boolean(),
  canManageCatches: z.boolean(),
  canReadMembers: z.boolean(),
  canManageMembers: z.boolean(),
});
export type SaveGroupPositionInput = z.infer<typeof formSchema>;
const mapDefaultValues = (
  groupId: string,
  position?: IGroupPositionModel
): Partial<SaveGroupPositionInput> => {
  if (!position)
    return {
      canManageCatches: false,
      canManageGroup: false,
      canManageMembers: false,
      canReadCatches: true,
      canReadMembers: true,
      groupId,
    };
  return {
    id: position.id,
    groupId: position.groupId,
    name: position.name,
    canManageGroup: position.canManageGroup,
    canReadCatches: position.canReadCatches,
    canManageCatches: position.canManageCatches,
    canReadMembers: position.canReadMembers,
    canManageMembers: position.canManageMembers,
  };
};
export const GroupPositionModal: React.FC<{
  defaultValue?: IGroupPositionModel;
  closeModal: () => void;
  groupId: string;
}> = ({ defaultValue, closeModal, groupId }) => {
  const {
    data,
    mutate,
    error: mutationError,
    isLoading,
    reset,
  } = useSavePositionMutation();
  const {
    watch,
    handleSubmit,
    register,
    formState: { isDirty, errors: formError },
  } = useForm<SaveGroupPositionInput>({
    resolver: zodResolver(formSchema),
    defaultValues: mapDefaultValues(groupId, defaultValue),
  });

  const { enqueueSnackbar } = useSnackbar();
  const [allErrors, setAllErrors] = useState<
    | ApiException
    | FieldErrors<{
        name: string;
        groupId: string;
        canManageGroup: boolean;
        canReadCatches: boolean;
        canManageCatches: boolean;
        canReadMembers: boolean;
        canManageMembers: boolean;
        id?: number | null | undefined;
      }>
  >();
  const {
    canManageCatches,
    canManageGroup,
    canManageMembers,
    canReadCatches,
    canReadMembers,
  } = watch();
  const submitHandler = (values: SaveGroupPositionInput) => {
    reset();
    mutate(values);
  };
  useEffect(() => {
    if (data) {
      enqueueSnackbar("Position saved", { variant: "success" });
      closeModal();
    }
  }, [data, enqueueSnackbar, closeModal]);
  useEffect(() => {
    if (formError) setAllErrors(formError);
  }, [formError]);
  useEffect(() => {
    if (mutationError) setAllErrors(mutationError);
  }, [mutationError]);
  const isSaveDisabled = !isDirty || isLoading;
  return (
    <Dialog open onClose={closeModal} fullWidth maxWidth="sm" scroll="paper">
      <StyledDialogTitle>
        <Typography variant="h6">
          {defaultValue ? "Edit position" : "Create position"}
        </Typography>
      </StyledDialogTitle>
      <DialogContent dividers={true}>
        <form id="addGroupPositionForm" onSubmit={handleSubmit(submitHandler)}>
          <Grid
            container
            spacing={2}
            padding={2}
            width={"100%"}
            justifyContent="center"
            alignItems="center"
          >
            <Grid item width="100%">
              <TextField
                fullWidth
                {...register("name", { required: true })}
                label="Position name"
              />
            </Grid>
            <Grid item width="50%">
              <FormControlLabel
                control={
                  <Switch
                    {...register("canManageGroup", { required: true })}
                    checked={canManageGroup}
                    defaultChecked={canManageGroup}
                  />
                }
                label="can manage group"
              />
            </Grid>
            <Grid item width="50%">
              <FormControlLabel
                control={
                  <Switch
                    {...register("canManageCatches", { required: true })}
                    checked={canManageCatches}
                    defaultChecked={canManageCatches}
                  />
                }
                label="can manage catches"
              />
            </Grid>
            <Grid item width="50%">
              <FormControlLabel
                control={
                  <Switch
                    {...register("canManageMembers", { required: true })}
                    checked={canManageMembers}
                    defaultChecked={canManageMembers}
                  />
                }
                label="can manage members"
              />
            </Grid>
            <Grid item width="50%">
              <FormControlLabel
                control={
                  <Switch
                    {...register("canReadMembers", { required: true })}
                    checked={canReadMembers}
                    defaultChecked={canReadMembers}
                  />
                }
                label="can read members"
              />
            </Grid>
            <Grid item>
              <FormControlLabel
                control={
                  <Switch
                    {...register("canReadCatches", { required: true })}
                    checked={canReadCatches}
                    defaultChecked={canReadCatches}
                  />
                }
                label="can read catches"
              />
            </Grid>
            {allErrors instanceof Error && (
              <Grid item width={"100%"}>
                <Alert severity="error">{allErrors.message}</Alert>
              </Grid>
            )}
            {!(allErrors instanceof ApiException) &&
              allErrors?.root?.message && (
                <Grid item width={"100%"}>
                  <Alert severity="error">{allErrors.root.message}</Alert>
                </Grid>
              )}
          </Grid>
        </form>
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
            </Grid>
          </Grid>
          <Grid
            item
            width="40%"
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
            <Button
              form="addGroupPositionForm"
              type="submit"
              variant="contained"
              disabled={isSaveDisabled}
            >
              Save
            </Button>
          </Grid>
        </Grid>
      </DialogActions>
    </Dialog>
  );
};
