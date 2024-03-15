import { z } from "zod";
import { GroupMemberModel } from "../../models/GroupMemberModel";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { Controller, FieldErrors, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSnackbar } from "notistack";
import { useSaveMemberMutation } from "./hooks/SaveMemberMutation";
import { useEffect, useState } from "react";
import { UserModel } from "../../models/UserModel";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  FormGroup,
  Grid,
  Typography,
} from "@mui/material";
import { StyledDialogTitle } from "../../common/StyledDialogTitle";
import { UsersSearch } from "../../common/UsersSearch";
import { PositionSearchBar } from "./PositionSearchBar";
import { ApiException } from "../../common/ApiException";

const formSchema = z.object({
  id: z.number().optional(),
  groupId: z.string(),
  userId: z.string(),
  positionId: z.number(),
});

export type SaveGroupMemberInput = z.infer<typeof formSchema>;

const mapDefaultValues = (
  groupId: string,
  values?: GroupMemberModel
): Partial<SaveGroupMemberInput> => {
  if (!values) return { groupId };
  return {
    id: values.id,
    groupId: values.groupId,
    userId: values.userId,
    positionId: values.positionId,
  };
};

export const AddMemberModal: React.FC<{
  existingMemberIds: string[];
  defaultValue?: GroupMemberModel;
  positions: GroupPositionModel[];
  groupId: string;
  closeModal: () => void;
}> = ({ defaultValue, positions, groupId, existingMemberIds, closeModal }) => {
  const {
    handleSubmit,
    control,
    watch,
    formState: { isDirty, errors: formError },
  } = useForm<SaveGroupMemberInput>({
    resolver: zodResolver(formSchema),
    defaultValues: mapDefaultValues(groupId, defaultValue),
  });
  const {
    mutate,
    data,
    reset,
    isLoading,
    error: mutationError,
  } = useSaveMemberMutation();
  const { enqueueSnackbar } = useSnackbar();
  const [allErrors, setAllErrors] = useState<
    | ApiException
    | FieldErrors<{
        groupId: string;
        userId: string;
        positionId: number;
        id?: number | undefined;
      }>
  >();
  const [chosenPosition, setChosenPosition] = useState<
    GroupPositionModel | undefined
  >(defaultValue?.position ?? undefined);
  const [chosenUser, setChosenUser] = useState<
    Omit<UserModel, "email"> | undefined
  >(defaultValue?.user as any);
  const { positionId, userId } = watch();
  const isSaveDisabled = !isDirty || isLoading || !positionId || !userId;
  useEffect(() => {
    if (formError) setAllErrors(formError);
  }, [formError]);
  useEffect(() => {
    if (mutationError) setAllErrors(mutationError);
  }, [mutationError]);
  useEffect(() => {
    if (data) {
      enqueueSnackbar("Member saved", { variant: "success" });
      closeModal();
    }
  }, [data, enqueueSnackbar, closeModal]);
  return (
    <Dialog open onClose={closeModal} fullWidth maxWidth="sm" scroll="paper">
      <StyledDialogTitle>
        <Typography variant="h6">
          {defaultValue ? "Edit member" : "Create member"}
        </Typography>
      </StyledDialogTitle>
      <DialogContent dividers>
        <form
          id="addGroupMemberForm"
          onSubmit={handleSubmit((value) => {
            reset();
            mutate(value);
          })}
        >
          <Grid
            container
            spacing={2}
            padding={2}
            width={"100%"}
            justifyContent="center"
            alignItems="center"
          >
            <Grid item width={"50%"}>
              <FormGroup>
                <Controller
                  control={control}
                  name="userId"
                  render={({ field: { onChange } }) => {
                    return (
                      <UsersSearch
                        onChange={(person) => {
                          setChosenUser(person);
                          onChange(person!.id);
                        }}
                        filter={(val) => !existingMemberIds.includes(val.id!)}
                        value={chosenUser}
                      />
                    );
                  }}
                />
              </FormGroup>
            </Grid>
            <Grid item width={"50%"}>
              <FormGroup>
                <Controller
                  control={control}
                  name="positionId"
                  render={({ field: { onChange } }) => {
                    return (
                      <PositionSearchBar
                        positions={positions}
                        position={chosenPosition}
                        onChange={(position) => {
                          onChange(position?.id!);
                          setChosenPosition(position);
                        }}
                      />
                    );
                  }}
                />
              </FormGroup>
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
            sx={{ display: "flex", justifyContent: "flex-start" }}
          >
            <Button
              form="addGroupMemberForm"
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
