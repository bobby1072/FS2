import { z } from "zod";
import imageCompression from "browser-image-compression";
import { base64StringToJpegFile } from "../../utils/StringUtils";
import { IGroupCatchModel } from "../../models/IGroupCatchModel";
import { useSaveCatchMutation } from "./hooks/SaveCatchMutation";
import { FieldErrors, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useSnackbar } from "notistack";
import { useEffect, useState } from "react";
import { ApiException } from "../../common/ApiException";
import { FormGroup, Grid, IconButton, TextField } from "@mui/material";
import { faker } from "@faker-js/faker";
import { useWorldFishFindSomeLikeMutation } from "./hooks/WorldFishFindSomeLike";
import { IWorldFishModel } from "../../models/IWorldFishModel";
import { Mention, MentionsInput } from "react-mentions";
import { Close } from "@mui/icons-material";
import { ErrorComponent } from "../../common/ErrorComponent";

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

  "&singleLine": {
    display: "inline-block",
    width: 180,

    highlighter: {
      padding: 1,
      border: "2px inset transparent",
    },
    input: {
      padding: 1,
      border: "2px inset",
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
    setValue,
    watch,
    formState: { errors: formErrors, isDirty: isFormDirty },
  } = useForm<SaveCatchInput>({
    defaultValues: mapDefaultValues(groupCatch),
    resolver: zodResolver(formSchema),
  });
  const { enqueueSnackbar } = useSnackbar();
  const [addedCatchPhoto, setAddedCatchPhoto] = useState<string | File>();
  const [allErrors, setAllErrors] = useState<ApiException | FieldErrors<any>>();
  const { catchPhoto, species } = watch();
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
  const {
    data: worldFishLike,
    mutate: fireWorldFishLike,
    isLoading: worldFishLikeLoading,
  } = useWorldFishFindSomeLikeMutation();
  useEffect(() => {
    if (fishSearchTerm?.length > 1) {
      fireWorldFishLike({ fishAnyname: fishSearchTerm });
    }
  }, [fishSearchTerm, fireWorldFishLike]);
  useEffect(() => {
    if (savedCatchId && useSnackBarOnSuccess)
      enqueueSnackbar(`New catch saved: ${savedCatchId}`, {
        variant: "success",
      });
  }, [savedCatchId, useSnackBarOnSuccess, enqueueSnackbar]);
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
        padding={2}
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
                        setFishSearchTerm("");
                        setValue("species", "");
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
                singleLine={true}
                allowSpaceInQuery
                style={inputStyle}
                allowSuggestionsAboveCursor
                onChange={(e) => {
                  const totalInput = e.target.value;
                  if (totalInput) {
                    setFishSearchTerm(totalInput);
                    setValue("species", totalInput);
                  } else {
                    setFishSearchTerm("");
                    setValue("species", "");
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
                    setValue("species", `%${id}%`);
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
        <Grid item width="100%">
          <ErrorComponent error={formErrors || mutationError} />
        </Grid>
      </Grid>
    </form>
  );
};
