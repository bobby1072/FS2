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
  Autocomplete,
  Button,
  FormGroup,
  Grid,
  IconButton,
  TextField,
  Typography,
} from "@mui/material";
import { faker } from "@faker-js/faker";
import { useWorldFishFindSomeLikeMutation } from "./hooks/WorldFishFindSomeLike";
import { IWorldFishModel } from "../../models/IWorldFishModel";
import { Close } from "@mui/icons-material";
import { ErrorComponent } from "../../common/ErrorComponent";
import { ApiException } from "../../common/ApiException";
import { getPrettyWorldFish } from "../../common/GetPrettyWorldFish";

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
export const mapDefaultValues = (
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
    caughtAt: groupCatch.caughtAt,
    createdAt: groupCatch.createdAt,
    catchPhoto: groupCatch.catchPhoto?.toString(),
  };
};

export const SaveGroupCatchForm: React.FC<{
  useSnackBarOnSuccess?: boolean;
  groupCatch?: IGroupCatchModel;
  groupId: string;
  closeForm?: () => void;
}> = ({ groupCatch, useSnackBarOnSuccess, groupId, closeForm }) => {
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
  const { catchPhoto, species, weight, length, latitude, longitude, caughtAt } =
    watch();
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
  const [speciesLocked, setSpeciesLocked] = useState<boolean>(
    groupCatch?.id ? true : false
  );
  const [worldFishOptions, setWorldFishOptions] = useState<IWorldFishModel[]>(
    []
  );
  const [fishSearchTerm, setFishSearchTerm] = useState<string>("");
  const [allErrors, setAllErrors] = useState<ApiException | FieldErrors<any>>();
  useEffect(() => {
    setAllErrors(mutationError || formErrors);
  }, [mutationError, formErrors]);
  const {
    data: worldFishLike,
    mutate: fireWorldFishLike,
    isLoading: worldFishLikeLoading,
  } = useWorldFishFindSomeLikeMutation();
  const clearSpeciesSearch = () => {
    setFishSearchTerm("");
    setValue("species", "");
    setWorldFishOptions([]);
    setValue("worldFishTaxocode", undefined);
  };
  useEffect(() => {
    if (fishSearchTerm?.length > 1) {
      fireWorldFishLike({ fishAnyname: fishSearchTerm });
    }
  }, [fishSearchTerm, fireWorldFishLike]);
  useEffect(() => {
    if (savedCatchId && useSnackBarOnSuccess) {
      enqueueSnackbar(`New catch saved: ${savedCatchId}`, {
        variant: "success",
      });
      closeForm?.();
    }
  }, [savedCatchId, useSnackBarOnSuccess, enqueueSnackbar, closeForm]);
  useEffect(() => {
    if (worldFishLike) {
      setWorldFishOptions(worldFishLike);
    }
  }, [worldFishLike]);
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
          <FormGroup>
            {speciesLocked ? (
              <TextField
                label="Species"
                disabled
                value={species}
                variant="outlined"
                InputProps={{
                  endAdornment: (
                    <IconButton
                      color="inherit"
                      size="small"
                      onClick={() => {
                        clearSpeciesSearch();
                        setSpeciesLocked(false);
                      }}
                    >
                      <Close fontSize="inherit" />
                    </IconButton>
                  ),
                }}
              />
            ) : (
              <Autocomplete
                options={worldFishOptions.filter((x) => x.englishName)}
                getOptionLabel={getPrettyWorldFish}
                renderOption={(props, option) => (
                  <li {...props}>
                    <Grid container direction="column">
                      <Grid item>
                        <Typography>
                          <strong>{getPrettyWorldFish(option)}</strong>
                        </Typography>
                      </Grid>
                      <Grid item>
                        <Typography fontSize={12}>
                          {option.scientificName}
                        </Typography>
                      </Grid>
                    </Grid>
                  </li>
                )}
                onInputChange={(e, value, reason) => {
                  if (reason === "input" && e?.type === "change" && value) {
                    setFishSearchTerm(value);
                    setValue("species", value);
                  } else if (reason === "clear" || !value) {
                    clearSpeciesSearch();
                  }
                }}
                isOptionEqualToValue={(option, value) =>
                  option.taxocode! === value.taxocode!
                }
                renderInput={(params) => (
                  <TextField
                    {...params}
                    variant="outlined"
                    label={"Species"}
                    InputLabelProps={{ shrink: true }}
                    size="medium"
                  />
                )}
                noOptionsText={
                  worldFishLikeLoading
                    ? "Loading..."
                    : worldFishLike
                    ? "No results"
                    : "Start typing to search"
                }
                onChange={(_, option) => {
                  setValue("worldFishTaxocode", option?.taxocode.toString());
                  const foundWorldFish = worldFishOptions.find(
                    (x) => x.taxocode === option?.taxocode
                  )?.englishName;
                  if (foundWorldFish) {
                    setValue("species", foundWorldFish);
                  }
                  setSpeciesLocked(true);
                  setFishSearchTerm("");
                  setWorldFishOptions([]);
                }}
              />
            )}
          </FormGroup>
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Weight"
            fullWidth
            type="number"
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
      </Grid>
    </form>
  );
};
