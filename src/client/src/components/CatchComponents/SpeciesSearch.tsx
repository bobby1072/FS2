import { Close } from "@mui/icons-material";
import {
  Autocomplete,
  FormGroup,
  Grid,
  IconButton,
  TextField,
  Typography,
} from "@mui/material";
import { getPrettyWorldFishName } from "../../common/GetPrettyWorldFish";
import { useEffect, useState } from "react";
import { useWorldFishFindSomeLikeMutation } from "./hooks/WorldFishFindSomeLike";
import { IWorldFishModel } from "../../models/IWorldFishModel";
import { IGroupCatchModel } from "../../models/IGroupCatchModel";

export const SpeciesSearch: React.FC<{
  setSpecies?: (vals?: string) => void;
  setWorldFishTaxocode?: (vals?: string) => void;
  defaultValue?: IGroupCatchModel;
  speciesString?: string;
}> = ({ setSpecies, setWorldFishTaxocode, defaultValue, speciesString }) => {
  const [fishSearchTerm, setFishSearchTerm] = useState<string>("");
  const {
    data: worldFishLike,
    mutate: fireWorldFishLike,
    isLoading: worldFishLikeLoading,
  } = useWorldFishFindSomeLikeMutation();
  const [speciesLocked, setSpeciesLocked] = useState<boolean>(
    defaultValue?.id ? true : false
  );
  const [worldFishOptions, setWorldFishOptions] = useState<IWorldFishModel[]>(
    []
  );
  useEffect(() => {
    if (fishSearchTerm?.length > 1) {
      fireWorldFishLike({ fishAnyname: fishSearchTerm });
    }
  }, [fishSearchTerm, fireWorldFishLike]);
  useEffect(() => {
    if (worldFishLike) {
      setWorldFishOptions(worldFishLike);
    }
  }, [worldFishLike]);
  const clearSpeciesSearch = () => {
    setFishSearchTerm("");
    setSpecies?.("");
    setWorldFishOptions([]);
    setWorldFishTaxocode?.(undefined);
  };
  return (
    <FormGroup>
      {speciesLocked ? (
        <TextField
          label="Species"
          disabled
          value={speciesString}
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
          getOptionLabel={getPrettyWorldFishName}
          disablePortal
          renderOption={(props, option) => (
            <li {...props}>
              <Grid container direction="column">
                <Grid item>
                  <Typography>
                    <strong>{getPrettyWorldFishName(option)}</strong>
                  </Typography>
                </Grid>
                <Grid item>
                  <Typography fontSize={12}>{option.scientificName}</Typography>
                </Grid>
              </Grid>
            </li>
          )}
          onInputChange={(e, value, reason) => {
            if (reason === "input" && e?.type === "change" && value) {
              setFishSearchTerm(value);
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
            setWorldFishTaxocode?.(option?.taxocode.toString());
            const foundWorldFish = worldFishOptions.find(
              (x) => x.taxocode === option?.taxocode
            )?.englishName;
            if (foundWorldFish) {
              setSpecies?.(foundWorldFish);
            }
            setSpeciesLocked(true);
            setFishSearchTerm("");
            setWorldFishOptions([]);
          }}
        />
      )}
    </FormGroup>
  );
};
