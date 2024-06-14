import {
  FormControl,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import { MapControlBox } from "../MapComponents/MapControlBox";
import { SpeciesSearch } from "../../common/SpeciesSearch";
import { Close } from "@mui/icons-material";
import { DatePicker } from "@mui/x-date-pickers";

export enum MapType {
  heatmap = "Heatmap",
  markerCluster = "Marker cluster",
}
export const CatchesMapSetFilterBox: React.FC<{
  mapFilters: {
    mapType?: MapType;
    minWeight: number;
    minLength: number;
    startDate?: Date;
    endDate?: Date;
  };
  setMapFilters: (vals: {
    mapType?: MapType;
    minWeight: number;
    minLength: number;
    startDate?: Date;
    endDate?: Date;
  }) => void;
  speciesFilter?: string;
  setSpeciesFilter: (species?: string) => void;
  earliestDate?: Date;
  latestDate?: Date;
}> = ({
  setMapFilters,
  mapFilters: { minLength, minWeight, endDate, mapType, startDate },
  setSpeciesFilter,
  speciesFilter,
  earliestDate,
  latestDate,
}) => {
  return (
    <MapControlBox>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        padding={1}
        spacing={1}
      >
        <Grid item width="95%">
          <SpeciesSearch
            setSpecies={setSpeciesFilter}
            speciesString={speciesFilter}
          />
        </Grid>
        <Grid item width="95%">
          {mapType ? (
            <TextField
              variant="outlined"
              label="Map type"
              disabled
              fullWidth
              InputProps={{
                endAdornment: (
                  <IconButton
                    color="inherit"
                    size="small"
                    onClick={() =>
                      setMapFilters({
                        minWeight,
                        mapType: undefined,
                        endDate,
                        startDate,
                        minLength,
                      })
                    }
                  >
                    <Close fontSize="inherit" />
                  </IconButton>
                ),
              }}
              value={mapType.toString()}
            />
          ) : (
            <FormControl fullWidth>
              <InputLabel>Map type</InputLabel>
              <Select
                fullWidth
                MenuProps={{
                  style: { zIndex: 5001 },
                }}
                value={mapType}
                endAdornment={
                  mapType && (
                    <InputAdornment position="end" sx={{ padding: 1 }}>
                      <IconButton
                        color="inherit"
                        size="small"
                        onClick={() =>
                          setMapFilters({
                            minWeight,
                            mapType: undefined,
                            endDate,
                            startDate,
                            minLength,
                          })
                        }
                      >
                        <Close fontSize="inherit" />
                      </IconButton>
                    </InputAdornment>
                  )
                }
                onChange={(v) => {
                  setMapFilters({
                    mapType: v.target.value as MapType,
                    minWeight,
                    endDate,
                    startDate,
                    minLength,
                  });
                }}
                label="Map type"
              >
                <MenuItem value={MapType.heatmap}>Heatmap</MenuItem>
                <MenuItem value={MapType.markerCluster}>
                  Marker cluster
                </MenuItem>
              </Select>
            </FormControl>
          )}
        </Grid>
        <Grid item width="95%">
          <TextField
            fullWidth
            variant="outlined"
            label="Minimum weight"
            type="number"
            InputLabelProps={{ shrink: true }}
            InputProps={{
              endAdornment: <InputAdornment position="end">lbs</InputAdornment>,
            }}
            onChange={(e) =>
              setMapFilters({
                mapType,
                minWeight: Number(e.target.value),
                endDate,
                startDate,
                minLength,
              })
            }
            value={minWeight}
          />
        </Grid>
        <Grid item width="95%">
          <TextField
            fullWidth
            variant="outlined"
            label="Minimum length"
            type="number"
            InputLabelProps={{ shrink: true }}
            InputProps={{
              endAdornment: <InputAdornment position="end">cm</InputAdornment>,
            }}
            onChange={(e) =>
              setMapFilters({
                mapType,
                minWeight,
                endDate,
                startDate,
                minLength: Number(e.target.value),
              })
            }
            value={minLength}
          />
        </Grid>
        <Grid item width={"95%"}>
          <DatePicker
            value={startDate}
            onChange={(date) =>
              setMapFilters({
                mapType,
                minWeight,
                startDate: date ?? undefined,
                endDate,
                minLength,
              })
            }
            label="Start date"
            minDate={earliestDate}
            maxDate={endDate ?? latestDate}
            slotProps={{
              popper: {
                disablePortal: true,
              },
              textField: {
                fullWidth: true,
                InputLabelProps: { shrink: true },
                onKeyDown: (e: any) => e.preventDefault(),
              },
            }}
          />
        </Grid>
        <Grid item width={"95%"}>
          <DatePicker
            value={endDate}
            onChange={(date) =>
              setMapFilters({
                mapType,
                minWeight,
                startDate,
                endDate: date ?? undefined,
                minLength,
              })
            }
            label="End date"
            minDate={startDate ?? earliestDate}
            maxDate={latestDate}
            slotProps={{
              popper: {
                disablePortal: true,
              },
              textField: {
                fullWidth: true,
                InputLabelProps: { shrink: true },
                onKeyDown: (e: any) => e.preventDefault(),
              },
            }}
          />
        </Grid>
      </Grid>
    </MapControlBox>
  );
};
