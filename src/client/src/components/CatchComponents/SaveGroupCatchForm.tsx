import { z } from "zod";
import imageCompression from "browser-image-compression";
import { base64StringToJpegFile } from "../../utils/StringUtils";
import { IGroupCatchModel } from "../../models/IGroupCatchModel";
import { useSaveCatchMutation } from "./hooks/SaveCatchMutation";
import { FieldErrors, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSnackbar } from "notistack";
import { useState } from "react";
import { ApiException } from "../../common/ApiException";
import { Grid } from "@mui/material";
import { faker } from "@faker-js/faker";

const formSchema = z.object({
  id: z.string().optional().nullable(),
  groupId: z.string(),
  species: z.string(),
  worldFishTaxocode: z.string().optional().nullable(),
  weight: z.number(),
  length: z.number(),
  description: z.string().optional().nullable(),
  latitude: z.number(),
  longitude: z.number(),
  caughtAt: z.string().datetime(),
  createdAt: z.string().datetime().optional().nullable(),
  catchPhoto: z.string().optional().nullable(),
});
const mapValuesToFormData = async (
  values: SaveCatchInput,
  newCatchPhoto?: File
): Promise<FormData> => {
  const formData = new FormData();
  if (values.id) formData.append("id", values.id);
  formData.append("groupId", values.groupId);
  formData.append("species", values.species);
  formData.append("worldFishTaxocode", values.worldFishTaxocode ?? "");
  formData.append("weight", values.weight.toString());
  formData.append("length", values.length.toString());
  formData.append("description", values.description ?? "");
  formData.append("latitude", values.latitude.toString());
  formData.append("longitude", values.longitude.toString());
  formData.append("caughtAt", new Date(values.caughtAt).toISOString());
  if (newCatchPhoto) {
    formData.append(
      "catchPhoto",
      await imageCompression(
        new File(
          [newCatchPhoto],
          `catchPhoto${values.id ?? faker.string.uuid()}.jpg`,
          {
            type: "image/jpeg",
          }
        ),
        { maxSizeMB: 2 }
      )
    );
  } else if (values.catchPhoto) {
    const file = base64StringToJpegFile(
      values.catchPhoto,
      `catchPhoto${values.id ?? faker.string.uuid()}.jpg`
    );
    formData.append(
      "catchPhoto",
      await imageCompression(file, { maxSizeMB: 2 })
    );
  }
  return formData;
};

export type SaveCatchInput = z.infer<typeof formSchema>;
const mapDefaultValues = (
  groupCatch?: IGroupCatchModel
): Partial<SaveCatchInput> => {
  if (!groupCatch) return {};
  return {
    id: groupCatch.id,
    groupId: groupCatch.groupId,
    species: groupCatch.species,
    worldFishTaxocode: groupCatch.worldFishTaxocode,
    weight: groupCatch.weight,
    length: groupCatch.length,
    description: groupCatch.description,
    latitude: groupCatch.latitude,
    longitude: groupCatch.longitude,
    caughtAt: groupCatch.caughtAt,
    createdAt: groupCatch.createdAt,
    catchPhoto: groupCatch.catchPhoto?.toString(),
  };
};

export const SaveGroupCatchForm: React.FC<{
  useSnackBarOnSuccess?: boolean;
  groupCatch?: IGroupCatchModel;
}> = ({ groupCatch, useSnackBarOnSuccess }) => {
  const {
    data: savedCatchId,
    mutate: saveCatchMutation,
    error: mutationError,
    reset: resetMutation,
    isLoading: isSavingCatch,
  } = useSaveCatchMutation();
  const {
    handleSubmit,
    register,
    watch,
    formState: { errors: formErros, isDirty: isformDirty },
  } = useForm<SaveCatchInput>({
    defaultValues: mapDefaultValues(groupCatch),
    resolver: zodResolver(formSchema),
  });
  const { enqueueSnackbar } = useSnackbar();
  const [addedCatchPhoto, setAddedCatchPhoto] = useState<string | File>();
  const [allErrors, setAllErrors] = useState<ApiException | FieldErrors<any>>();
  const { catchPhoto } = watch();
  const isDirty = isformDirty || catchPhoto !== groupCatch?.catchPhoto;
  const submitHandler = async (values: SaveCatchInput) => {
    resetMutation();
    saveCatchMutation(
      await mapValuesToFormData(
        values,
        typeof addedCatchPhoto === "string" ? undefined : addedCatchPhoto
      )
    );
  };
  return (
    <form id="saveCatchFrom" onSubmit={handleSubmit(submitHandler)}>
      <Grid
        container
        spacing={2}
        padding={2}
        width={"100%"}
        justifyContent="center"
        alignItems="center"
      ></Grid>
    </form>
  );
};
