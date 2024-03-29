import { z } from "zod";
import { IGroupModel } from "../../models/GroupModel";
import { useSaveGroupMutation } from "./hooks/SaveGroupMutation";
import { FieldErrors, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Alert,
  FormControlLabel,
  Grid,
  Switch,
  TextField,
  Tooltip,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useSnackbar } from "notistack";
import { ApiException } from "../../common/ApiException";
const formSchema = z.object({
  name: z.string(),
  description: z.string().optional().nullable(),
  isPublic: z.boolean(),
  isListed: z.boolean(),
  leaderId: z.string().optional().nullable(),
  emblem: z.string().optional().nullable(),
  id: z.string().optional().nullable(),
  createdAt: z.string().optional().nullable(),
});
export type SaveGroupInput = z.infer<typeof formSchema>;
const mapDefaultValues = (group?: IGroupModel): Partial<SaveGroupInput> => {
  if (!group) return { isListed: true, isPublic: true };
  return {
    id: group.id,
    name: group.name,
    description: group?.description,
    isPublic: group.public,
    leaderId: group.leaderId,
    createdAt: group.createdAt,
    isListed: group.listed,
    emblem: group?.emblem?.toString(),
  };
};
export const CreateGroupModalForm: React.FC<{
  group?: IGroupModel;
  setIsSaveDisabled?: (boolVal: boolean) => void;
  closeModal?: () => void;
  useSnackBarOnSuccess?: boolean;
}> = ({
  group,
  setIsSaveDisabled,
  useSnackBarOnSuccess = false,
  closeModal,
}) => {
  const {
    data: savedId,
    mutate: saveGroupMutation,
    error: mutationError,
    reset: resetMutation,
    isLoading: isSaving,
  } = useSaveGroupMutation();
  const {
    handleSubmit,
    register,
    watch,
    setValue,
    formState: { errors: formError, isDirty: isFormDirty },
  } = useForm<SaveGroupInput>({
    defaultValues: mapDefaultValues(group),
    resolver: zodResolver(formSchema),
  });
  const { enqueueSnackbar } = useSnackbar();
  const { isListed, isPublic, id, name, emblem } = watch();
  const [allErrors, setAllErrors] = useState<
    | ApiException
    | FieldErrors<{
        name: string;
        isPublic: boolean;
        isListed: boolean;
        description?: string | undefined;
        emblem?: string | undefined;
        id?: string | undefined;
      }>
  >();
  const isDirty = isFormDirty || group?.emblem !== emblem;
  useEffect(() => {
    if (savedId && useSnackBarOnSuccess)
      enqueueSnackbar(`New group id: ${savedId}`, { variant: "success" });
    if (savedId && closeModal) closeModal();
  }, [savedId, enqueueSnackbar, id, useSnackBarOnSuccess, closeModal]);
  useEffect(() => {
    setIsSaveDisabled?.(!isDirty || !name || isSaving);
  }, [isDirty, setIsSaveDisabled, name, isSaving]);
  useEffect(() => {
    if (formError) setAllErrors(formError);
  }, [formError]);
  useEffect(() => {
    if (mutationError) setAllErrors(mutationError);
  }, [mutationError]);
  const submitHandler = (values: SaveGroupInput) => {
    resetMutation();
    saveGroupMutation(values);
  };
  return (
    <form onSubmit={handleSubmit(submitHandler)} id="groupSaveForm">
      <Grid
        container
        spacing={2}
        padding={2}
        width={"100%"}
        justifyContent="center"
        alignItems="center"
      >
        <Grid item width="50%">
          <TextField
            {...register("name", { required: true })}
            label="Group name"
            fullWidth
            multiline
            rows={2}
          />
        </Grid>
        <Grid item width="50%">
          <TextField
            {...register("description", { required: false })}
            label="Group description"
            fullWidth
            multiline
            rows={2}
          />
        </Grid>
        <Grid
          item
          width="40%"
          sx={{ display: "flex", justifyContent: "flex-start" }}
        >
          <Tooltip title="Public groups are visible to everyone. Private groups are only visible to members and are invite only.">
            <FormControlLabel
              control={
                <Switch
                  checked={isPublic}
                  defaultChecked={isPublic}
                  {...register("isPublic", { required: false })}
                />
              }
              label="Public"
            />
          </Tooltip>
        </Grid>
        <Grid
          item
          width="40%"
          sx={{ display: "flex", justifyContent: "flex-end" }}
        >
          <Tooltip title="Listed groups are visible to everyone. Unlisted groups are never visible on the main page.">
            <FormControlLabel
              control={
                <Switch
                  checked={isListed}
                  defaultChecked={isListed}
                  {...register("isListed", { required: false })}
                />
              }
              label="Listed"
            />
          </Tooltip>
        </Grid>
        <Grid
          item
          width="100%"
          sx={{
            textAlign: "center",
          }}
        >
          <input
            type="file"
            accept="image/*"
            onChange={(e) => {
              const foundFile = e.target.files?.item(0);
              if (foundFile) {
                const reader = new FileReader();
                reader.onloadend = () => {
                  const result = reader.result as string;
                  setValue(
                    "emblem",
                    result.replace(/^data:image\/[^;]+;base64,/, "")
                  );
                };
                reader.readAsDataURL(foundFile);
              }
            }}
          />
        </Grid>
        {allErrors instanceof Error && (
          <Grid item width={"100%"}>
            <Alert severity="error">{allErrors.message}</Alert>
          </Grid>
        )}
        {!(allErrors instanceof ApiException) && allErrors?.root?.message && (
          <Grid item width={"100%"}>
            <Alert severity="error">{allErrors.root.message}</Alert>
          </Grid>
        )}
        {savedId && (
          <Grid item width={"100%"}>
            <Alert severity="success">Group saved!</Alert>
          </Grid>
        )}
      </Grid>
    </form>
  );
};
