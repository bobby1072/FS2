import { z } from "zod";
import imageCompression from "browser-image-compression";
import { base64StringToJpegFile } from "../../utils/StringUtils";
import { IGroupCatchModel } from "../../models/IGroupCatchModel";
import { useSaveCatchMutation } from "./hooks/SaveCatchMutation";
import { Controller, FieldErrors, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSnackbar } from "notistack";
import { DatePicker } from "@mui/x-date-pickers";
import { useEffect, useState } from "react";
import { Button, FormGroup, Grid, IconButton, TextField } from "@mui/material";
import { faker } from "@faker-js/faker";
import { useWorldFishFindSomeLikeMutation } from "./hooks/WorldFishFindSomeLike";
import { IWorldFishModel } from "../../models/IWorldFishModel";
import { Mention, MentionsInput } from "react-mentions";
import { Close } from "@mui/icons-material";
import { ErrorComponent } from "../../common/ErrorComponent";
import { ApiException } from "../../common/ApiException";
const inputStyle = {
  control: {
    backgroundColor: "#fff",
    fontSize: 14,
    fontWeight: "normal",
  },

  "&multiLine": {
    control: {
      fontFamily: "monospace",
      minHeight: 63,
    },
    highlighter: {
      padding: 9,
      border: "1px solid transparent",
    },
    input: {
      padding: 9,
      border: "1px solid silver",
    },
  },

  suggestions: {
    list: {
      backgroundColor: "white",
      border: "1px solid rgba(0,0,0,0.15)",
      fontSize: 14,
    },
    item: {
      padding: "5px 15px",
      borderBottom: "1px solid rgba(0,0,0,0.15)",
      "&focused": {
        backgroundColor: "#cee4e5",
      },
    },
  },
};

const formSchema = z.object({
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
const mapDefaultValues = (
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
  } = useForm<SaveCatchInput>({
    defaultValues: mapDefaultValues(groupId, groupCatch),
    resolver: zodResolver(formSchema),
  });
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
  const [speciesLocked, setSpeciesLocked] = useState<boolean>(false);
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
                        setSpeciesLocked(false);
                        clearSpeciesSearch();
                      }}
                    >
                      <Close fontSize="inherit" />
                    </IconButton>
                  ),
                }}
              />
            ) : (
              <MentionsInput
                value={species}
                placeholder="Type a species..."
                singleLine={false}
                allowSpaceInQuery
                style={inputStyle}
                allowSuggestionsAboveCursor
                onChange={(e) => {
                  const totalInput = e.target.value;
                  if (totalInput) {
                    setFishSearchTerm(totalInput);
                    setValue("species", totalInput);
                  } else {
                    clearSpeciesSearch();
                  }
                }}
              >
                <Mention
                  trigger={""}
                  isLoading={worldFishLikeLoading}
                  appendSpaceOnAdd
                  onAdd={(id) => {
                    setSpeciesLocked(true);
                    setFishSearchTerm("");
                    const foundWorldFish = worldFishOptions.find(
                      (x) => x.taxocode === id
                    )?.englishName;
                    if (foundWorldFish) {
                      setValue("species", foundWorldFish);
                    }
                    setValue("worldFishTaxocode", id.toString());
                    setWorldFishOptions([]);
                  }}
                  style={{ backgroundColor: "#EBEBEB" }}
                  data={worldFishOptions
                    .filter((x) => x.englishName)
                    .map((x) => ({
                      id: x.taxocode,
                      display: `${x.englishName}${
                        x.nickname ? ` (${x.nickname})` : ""
                      }`,
                    }))}
                />
              </MentionsInput>
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
                  <DatePicker
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
