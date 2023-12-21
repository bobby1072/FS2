import { z } from "zod";
import { GroupModel } from "../../models/GroupModel";
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
import { AxiosError } from "axios";
const formSchema = z.object({
  name: z.string(),
  description: z.string().optional(),
  isPublic: z.boolean(),
  isListed: z.boolean(),
  emblem: z.string().optional(),
  id: z.string().optional(),
});
export type SaveGroupInput = z.infer<typeof formSchema>;
const mapDefaultValues = (
  group?: GroupModel
): Partial<SaveGroupInput> | undefined => {
  if (!group) return { isListed: true, isPublic: true };
  return {
    id: group.id,
    name: group.name,
    description: group?.description,
    isPublic: group.Public,
    isListed: group.Listed,
    emblem: group?.emblem?.toString(),
  };
};
export const CreateGroupModalForm: React.FC<{
  group?: GroupModel;
  setIsDirty?: (boolVal: boolean) => void;
}> = ({ group, setIsDirty }) => {
  const {
    data,
    mutate,
    error: mutationError,
    reset: resetMutation,
  } = useSaveGroupMutation();
  const {
    handleSubmit,
    register,
    watch,
    setValue,
    formState: { errors: formError, isDirty },
  } = useForm<SaveGroupInput>({
    defaultValues: mapDefaultValues(group),
    resolver: zodResolver(formSchema),
  });
  const { isListed, isPublic } = watch();
  const [allErrors, setAllErrors] = useState<
    | AxiosError
    | FieldErrors<{
        name: string;
        isPublic: boolean;
        isListed: boolean;
        description?: string | undefined;
        emblem?: string | undefined;
        id?: string | undefined;
      }>
  >();
  useEffect(() => {
    setIsDirty?.(!isDirty);
  }, [isDirty, setIsDirty]);
  useEffect(() => {
    if (formError) setAllErrors(formError);
  }, [formError]);
  useEffect(() => {
    if (mutationError) setAllErrors(mutationError);
  }, [mutationError]);
  const submitHandler = (values: SaveGroupInput) => {
    resetMutation();
    mutate(values);
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
                reader.onload = () => {
                  const result = reader.result as string;
                  setValue("emblem", result);
                };
                reader.readAsDataURL(foundFile);
              }
            }}
          />
        </Grid>
        {allErrors instanceof AxiosError && (
          <Grid item width={"100%"}>
            <Alert severity="error">{allErrors.message}</Alert>
          </Grid>
        )}
        {!(allErrors instanceof AxiosError) && allErrors?.root?.message && (
          <Grid item width={"100%"}>
            <Alert severity="error">{allErrors.root.message}</Alert>
          </Grid>
        )}
        {data && (
          <Grid item width={"100%"}>
            <Alert severity="success">Group saved!</Alert>
          </Grid>
        )}
      </Grid>
    </form>
  );
};
