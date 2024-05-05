import { z } from "zod";
import imageCompression from "browser-image-compression";
import { base64StringToJpegFile } from "../../utils/StringUtils";
import { IGroupCatchModel } from "../../models/IGroupCatchModel";
import { useSaveCatchMutation } from "./hooks/SaveCatchMutation";
import { Controller, FieldErrors, useFormContext } from "react-hook-form";
import { useSnackbar } from "notistack";
import { DateTimePicker } from "@mui/x-date-pickers";
import { useEffect, useState } from "react";
import {
  Alert,
  Button,
  FormGroup,
  Grid,
  InputAdornment,
  TextField,
  Typography,
} from "@mui/material";
import { faker } from "@faker-js/faker";
import { ErrorComponent } from "../../common/ErrorComponent";
import { ApiException } from "../../common/ApiException";
import { SpeciesSearch } from "./SpeciesSearch";

export const formSchema = z.object({
  id: z.string().optional().nullable(),
  groupId: z.string(),
  species: z.string(),
  worldFishTaxocode: z.string().optional().nullable(),
  weight: z.string().transform((x) => Number(x)),
  length: z.string().transform((x) => Number(x)),
  description: z.string().optional().nullable(),
  latitude: z.string().transform((x) => Number(x)),
  longitude: z.string().transform((x) => Number(x)),
  caughtAt: z
    .string()
    .datetime()
    .transform((x) => new Date(x)),
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
  formData.append("caughtAt", values.createdAt ?? "");
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
export const mapDefaultValuesToSaveCatchInput = (
  groupId: string,
  groupCatch?: IGroupCatchModel
): Partial<SaveCatchInput> => {
  if (!groupCatch) return { groupId };
  return {
    id: groupCatch.id,
    groupId,
    species: groupCatch.species,
    worldFishTaxocode: groupCatch.worldFishTaxocode,
    weight: groupCatch.weight,
    length: groupCatch.length,
    description: groupCatch.description,
    latitude: groupCatch.latitude,
    longitude: groupCatch.longitude,
    caughtAt: new Date(groupCatch.caughtAt),
    createdAt: groupCatch.createdAt,
    catchPhoto: groupCatch.catchPhoto?.toString(),
  };
};

export const SaveGroupCatchForm: React.FC<{
  useSnackBarOnSuccess?: boolean;
  groupCatch?: IGroupCatchModel;
  showMapInfoMessage?: boolean;
  closeForm?: () => void;
}> = ({
  groupCatch,
  useSnackBarOnSuccess,
  closeForm,
  showMapInfoMessage = false,
}) => {
  const {
    data: savedCatchId,
    mutate: saveCatchMutation,
    error: mutationError,
    reset: resetMutation,
    isLoading: isSavingCatch,
  } = useSaveCatchMutation();
  const {
    handleSubmit,
    control,
    register,
    setValue,
    watch,
    formState: { errors: formErrors, isDirty: isFormDirty },
  } = useFormContext<SaveCatchInput>();
  const { enqueueSnackbar } = useSnackbar();
  const [addedCatchPhoto, setAddedCatchPhoto] = useState<string | File>();
  const allFieldValues = watch();
  const { catchPhoto, species, weight, length, latitude, longitude, caughtAt } =
    allFieldValues;
  const isDirty = isFormDirty || catchPhoto !== groupCatch?.catchPhoto;
  const submitHandler = async (values: SaveCatchInput) => {
    resetMutation();
    saveCatchMutation(
      await mapValuesToFormData(
        values,
        typeof addedCatchPhoto === "string" ? undefined : addedCatchPhoto
      )
    );
  };
  const [allErrors, setAllErrors] = useState<ApiException | FieldErrors<any>>();
  useEffect(() => {
    setAllErrors(mutationError || formErrors);
  }, [mutationError, formErrors]);
  useEffect(() => {
    if (savedCatchId && useSnackBarOnSuccess) {
      enqueueSnackbar(`New catch saved: ${savedCatchId}`, {
        variant: "success",
      });
      closeForm?.();
    }
  }, [savedCatchId, useSnackBarOnSuccess, enqueueSnackbar, closeForm]);
  return (
    <form id="saveCatchFrom" onSubmit={handleSubmit(submitHandler)}>
      <Grid
        container
        spacing={2}
        padding={1}
        width={"100%"}
        justifyContent="center"
        alignItems="center"
      >
        <Grid item width="25%">
          <SpeciesSearch
            defaultValue={groupCatch}
            speciesString={species}
            setSpecies={(value) => setValue("species", value)}
            setWorldFishTaxocode={(value) =>
              setValue("worldFishTaxocode", value)
            }
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Weight"
            fullWidth
            type="number"
            InputProps={{
              endAdornment: <InputAdornment position="end">kg</InputAdornment>,
            }}
            {...register("weight", { required: true })}
            error={!!formErrors?.weight}
            helperText={formErrors?.weight?.message}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Length"
            fullWidth
            type="number"
            {...register("length", { required: true })}
            InputProps={{
              endAdornment: <InputAdornment position="end">cm</InputAdornment>,
            }}
            error={!!formErrors?.length}
            helperText={formErrors?.length?.message}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Description"
            fullWidth
            multiline
            {...register("description")}
            error={!!formErrors?.description}
            helperText={formErrors?.description?.message}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Latitude"
            fullWidth
            value={latitude ?? ""}
            type="number"
            {...register("latitude", { required: true })}
            error={!!formErrors?.latitude}
            helperText={formErrors?.latitude?.message}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            type="number"
            label="Longitude"
            value={longitude ?? ""}
            fullWidth
            {...register("longitude", { required: true })}
            error={!!formErrors?.longitude}
            helperText={formErrors?.longitude?.message}
          />
        </Grid>
        <Grid item width="25%">
          <FormGroup>
            <Controller
              control={control}
              name="caughtAt"
              render={({ field }) => {
                return (
                  <DateTimePicker
                    label="Caught at"
                    inputRef={field.ref}
                    value={field.value ? new Date(field.value) : undefined}
                    onChange={(date) => {
                      field.onChange(date?.toISOString());
                    }}
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        required: true,
                        InputLabelProps: { shrink: true },
                        onKeyDown: (e: any) => e.preventDefault(),
                      },
                    }}
                  />
                );
              }}
            />
          </FormGroup>
        </Grid>
        <Grid
          item
          width="25%"
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
                setAddedCatchPhoto(foundFile);
              }
            }}
          />
        </Grid>
        <Grid item width="100%">
          <Button
            type="submit"
            variant="contained"
            color="primary"
            fullWidth
            disabled={
              !isDirty ||
              isSavingCatch ||
              Object.values(formErrors).some((x) => !!x) ||
              !species ||
              !weight ||
              !length ||
              !latitude ||
              !longitude ||
              !caughtAt
            }
          >
            Save catch
          </Button>
        </Grid>
        {allErrors && (
          <Grid item width="100%">
            <ErrorComponent error={allErrors} />
          </Grid>
        )}
        {!Object.values(formErrors).some((x) => !!x) && showMapInfoMessage && (
          <Grid item width="100%">
            <Alert severity="info">
              <Typography>
                Click on the map to set the latitude and longitude
              </Typography>
            </Alert>
          </Grid>
        )}
      </Grid>
    </form>
  );
};
