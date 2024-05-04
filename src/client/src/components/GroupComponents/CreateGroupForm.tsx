import { z } from "zod";
import { IGroupModel } from "../../models/IGroupModel";
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
import { ErrorComponent } from "../../common/ErrorComponent";
import imageCompression from "browser-image-compression";
import { base64StringToJpegFile } from "../../utils/StringUtils";
import { faker } from "@faker-js/faker";
const formSchema = z.object({
  name: z.string(),
  description: z.string().optional().nullable(),
  isPublic: z.boolean(),
  isListed: z.boolean(),
  catchesPublic: z.boolean(),
  leaderId: z.string().optional().nullable(),
  emblem: z.string().optional().nullable(),
  id: z.string().optional().nullable(),
  createdAt: z.string().datetime().optional().nullable(),
});
const mapValuesToFormData = async (
  values: SaveGroupInput,
  newEmblem?: File
): Promise<FormData> => {
  const formData = new FormData();
  if (values.id) formData.append("id", values.id);
  if (values.leaderId) formData.append("leaderId", values.leaderId);
  formData.append("name", values.name);
  formData.append("description", values.description ?? "");
  formData.append("isPublic", values.isPublic.toString());
  formData.append("isListed", values.isListed.toString());
  formData.append("catchesPublic", values.catchesPublic.toString());
  formData.append(
    "createdAt",
    values.createdAt ? new Date(values.createdAt).toISOString() : ""
  );
  if (newEmblem) {
    formData.append(
      "emblem",
      await imageCompression(
        new File(
          [newEmblem],
          `groupEmblem${values.id ?? faker.string.uuid()}.jpg`,
          {
            type: "image/jpeg",
          }
        ),
        { maxSizeMB: 1, initialQuality: 0.5 }
      )
    );
  } else if (values.emblem) {
    const file = base64StringToJpegFile(
      values.emblem,
      `groupEmblem${values.id ?? faker.string.uuid()}.jpg`
    );
    formData.append(
      "emblem",
      await imageCompression(file, { maxSizeMB: 1, initialQuality: 0.5 })
    );
  }
  return formData;
};

export type SaveGroupInput = z.infer<typeof formSchema>;
const mapDefaultValues = (group?: IGroupModel): Partial<SaveGroupInput> => {
  if (!group) return { isListed: true, isPublic: true };
  return {
    id: group.id,
    name: group.name,
    description: group?.description,
    catchesPublic: group.catchesPublic,
    isPublic: group.public,
    leaderId: group.leaderId,
    createdAt: group.createdAt,
    isListed: group.listed,
    emblem: group.emblem?.toString(),
  };
};
export const CreateGroupForm: React.FC<{
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
    formState: { errors: formError, isDirty: isFormDirty },
  } = useForm<SaveGroupInput>({
    defaultValues: mapDefaultValues(group),
    resolver: zodResolver(formSchema),
  });
  const { enqueueSnackbar } = useSnackbar();
  const [addedEmblem, setAddedEmblem] = useState<string | File>();
  const { isListed, catchesPublic, isPublic, id, name, emblem } = watch();
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
    if (emblem) {
      setAddedEmblem(emblem);
    } else if (group?.emblem) {
      setAddedEmblem(group?.emblem);
    }
  }, [group?.emblem, emblem]);
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
  const submitHandler = async (values: SaveGroupInput) => {
    resetMutation();
    saveGroupMutation(
      await mapValuesToFormData(
        values,
        typeof addedEmblem === "string" ? undefined : addedEmblem
      )
    );
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
            error={!!formError?.name}
            helperText={formError?.name?.message}
          />
        </Grid>
        <Grid item width="50%">
          <TextField
            {...register("description", { required: false })}
            label="Group description"
            fullWidth
            multiline
            rows={2}
            error={!!formError?.description}
            helperText={formError?.description?.message}
          />
        </Grid>
        <Grid
          item
          width="30%"
          sx={{ display: "flex", justifyContent: "flex-start" }}
        >
          <Tooltip title="This means group catches are visible to everyone.">
            <FormControlLabel
              control={
                <Switch
                  checked={catchesPublic}
                  defaultChecked={catchesPublic}
                  {...register("catchesPublic", { required: false })}
                />
              }
              label="Catches public"
            />
          </Tooltip>
        </Grid>
        <Grid
          item
          width="30%"
          sx={{ display: "flex", justifyContent: "flex-start" }}
        >
          <Tooltip title="Public groups are free for everyone to look at. Private groups are invite only.">
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
          width="30%"
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
                setAddedEmblem(foundFile);
              }
            }}
          />
        </Grid>
        <Grid item width={"100%"}>
          <ErrorComponent error={allErrors} />
        </Grid>
        {savedId && (
          <Grid item width={"100%"}>
            <Alert severity="success">Group saved!</Alert>
          </Grid>
        )}
      </Grid>
    </form>
  );
};
