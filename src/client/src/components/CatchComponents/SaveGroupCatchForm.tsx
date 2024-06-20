import { z } from "zod";
import imageCompression from "browser-image-compression";
import { base64StringToJpegFile } from "../../utils/StringUtils";
import { IGroupCatchModel } from "../../models/IGroupCatchModel";
import { useSaveCatchMutation } from "./hooks/SaveCatchMutation";
import {
  Controller,
  FieldErrors,
  useForm,
  useFormContext,
} from "react-hook-form";
import { useSnackbar } from "notistack";
import { DateTimePicker } from "@mui/x-date-pickers";
import { useEffect, useState } from "react";
import {
  Alert,
  Button,
  Grid,
  InputAdornment,
  TextField,
  Typography,
} from "@mui/material";
import { faker } from "@faker-js/faker";
import { ErrorComponent } from "../../common/ErrorComponent";
import { ApiException } from "../../common/ApiException";
import { zodResolver } from "@hookform/resolvers/zod";
import { SpeciesSearch } from "../../common/SpeciesSearch";
export const formSchema = z.object({
  id: z.string().uuid().optional().nullable(),
  groupId: z.string().uuid(),
  species: z.string(),
  worldFishTaxocode: z.string().optional().nullable(),
  weight: z.string().optional().nullable(),
  length: z.string().optional().nullable(),
  description: z.string().optional().nullable(),
  latitude: z.string(),
  longitude: z.string(),
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
  formData.append("weight", values.weight!.toString());
  formData.append("length", values.length!.toString());
  formData.append("description", values.description ?? "");
  formData.append("latitude", values.latitude.toString());
  formData.append("longitude", values.longitude.toString());
  formData.append("caughtAt", new Date(values.caughtAt).toISOString());
  formData.append("createdAt", values.createdAt ?? "");
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
        { maxSizeMB: 1 }
      )
    );
  } else if (values.catchPhoto) {
    const file = base64StringToJpegFile(
      values.catchPhoto,
      `catchPhoto${values.id ?? faker.string.uuid()}.jpg`
    );
    formData.append(
      "catchPhoto",
      await imageCompression(file, { maxSizeMB: 1 })
    );
  }
  return formData;
};
const useFormContextOrFresh = (defaultValues?: IGroupCatchModel) => {
  const form = useForm<SaveCatchInput>({
    defaultValues: mapDefaultValuesToSaveCatchInput(
      defaultValues?.groupId ?? "",
      defaultValues
    ),
    resolver: zodResolver(formSchema),
  });
  try {
    const formContext = useFormContext<SaveCatchInput>();
    if (formContext) return formContext;
    return form;
  } catch (e) {
    return form;
  }
};
export type SaveCatchInput = z.infer<typeof formSchema>;
export const mapDefaultValuesToSaveCatchInput = (
  groupId?: string,
  groupCatch?: IGroupCatchModel
): Partial<SaveCatchInput> => {
  if (!groupCatch) return { groupId };
  return {
    id: groupCatch.id,
    groupId,
    species: groupCatch.species,
    worldFishTaxocode: groupCatch.worldFishTaxocode,
    weight: groupCatch.weight.toString(),
    length: groupCatch.length.toString(),
    description: groupCatch.description,
    latitude: groupCatch.latitude.toString(),
    longitude: groupCatch.longitude.toString(),
    caughtAt: groupCatch.caughtAt,
    createdAt: groupCatch.createdAt,
    catchPhoto: groupCatch.catchPhoto?.toString(),
  };
};
const setNumberValue = (
  e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  setValue: (num: string) => void
) => {
  if (e.target.value === "") {
    setValue(e.target.value);
    return;
  }
  if (isNaN(Number(e.target.value))) return;
  setValue(e.target.value);
};

const today = new Date();
export const SaveGroupCatchForm: React.FC<{
  useSnackBarOnSuccess?: boolean;
  groupCatch?: IGroupCatchModel;
  showMapInfoMessage?: boolean;
  closeForm?: () => void;
}> = ({
  groupCatch,
  useSnackBarOnSuccess = false,
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
    setValue,
    watch,
    formState: { errors: formErrors, isDirty: isFormDirty },
  } = useFormContextOrFresh(groupCatch);
  const { enqueueSnackbar } = useSnackbar();
  const [addedCatchPhoto, setAddedCatchPhoto] = useState<string | File>();
  const [looseDescriptionState, setLooseDescriptionState] = useState<
    string | undefined
  >(groupCatch?.description ?? undefined);
  const [looseWeightState, setLooseWeightState] = useState<string | undefined>(
    groupCatch?.weight.toString()
  );
  const [looseLengthState, setLooseLengthState] = useState<string | undefined>(
    groupCatch?.length.toString()
  );
  const allFieldValues = watch();
  const { species, latitude, longitude, caughtAt } = allFieldValues;
  const isDirty =
    isFormDirty ||
    (looseWeightState?.toString() !== groupCatch?.weight.toString() &&
      looseLengthState) ||
    (looseWeightState?.toString() !== groupCatch?.length.toString() &&
      looseLengthState);
  const submitHandler = async (values: SaveCatchInput) => {
    resetMutation();
    saveCatchMutation(
      await mapValuesToFormData(
        {
          ...values,
          description: looseDescriptionState,
          weight: looseWeightState!,
          length: looseLengthState!,
        },
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
    <form id="saveCatchForm" onSubmit={handleSubmit(submitHandler)}>
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
            setSpecies={(value) =>
              setValue("species", value ?? "", { shouldDirty: true })
            }
            setWorldFishTaxocode={(value) =>
              setValue("worldFishTaxocode", value, { shouldDirty: true })
            }
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Weight"
            fullWidth
            onChange={(e) => {
              setNumberValue(e, (s) => setLooseWeightState(s));
            }}
            value={looseWeightState ?? ""}
            InputProps={{
              endAdornment: <InputAdornment position="end">lbs</InputAdornment>,
              required: true,
            }}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Length"
            fullWidth
            onChange={(e) => {
              setNumberValue(e, (s) => setLooseLengthState(s));
            }}
            value={looseLengthState ?? ""}
            InputProps={{
              endAdornment: <InputAdornment position="end">cm</InputAdornment>,
              required: true,
            }}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Description"
            fullWidth
            multiline
            value={looseDescriptionState ?? ""}
            onChange={(e) => {
              setLooseDescriptionState(e.target.value);
            }}
          />
        </Grid>
        <Grid item width="25%">
          <TextField
            label="Latitude"
            fullWidth
            value={latitude ?? ""}
            onChange={(e) => {
              setNumberValue(e, (s) =>
                setValue("latitude", s, { shouldDirty: true })
              );
            }}
            InputProps={{ required: true }}
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
            InputProps={{ required: true }}
            onChange={(e) => {
              setNumberValue(e, (s) =>
                setValue("longitude", s, { shouldDirty: true })
              );
            }}
            error={!!formErrors?.longitude}
            helperText={formErrors?.longitude?.message}
          />
        </Grid>
        <Grid item width="25%">
          <Controller
            control={control}
            name="caughtAt"
            render={({ field }) => {
              return (
                <DateTimePicker
                  maxDateTime={today}
                  label="Caught at"
                  inputRef={field.ref}
                  value={field.value ? new Date(field.value) : undefined}
                  onChange={(date) => {
                    field.onChange(date?.toISOString());
                  }}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      InputLabelProps: { shrink: true },
                      onKeyDown: (e: any) => e.preventDefault(),
                    },
                  }}
                />
              );
            }}
          />
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
              !species ||
              !looseWeightState ||
              !looseLengthState ||
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
        {!isSavingCatch &&
          allErrors &&
          !Object.values(allErrors).some((x) => !!x) &&
          showMapInfoMessage && (
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
