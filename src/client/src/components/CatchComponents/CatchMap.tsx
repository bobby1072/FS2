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
import { useGetAllPartialCatchesForGroupQuery } from "./hooks/GetAllPartialCatchesForGroup";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../../common/contexts/AbilitiesContext";
import { useParams } from "react-router-dom";
import { useState } from "react";
import { SaveCatchInput } from "./SaveGroupCatchForm";
import { UseFormReturn } from "react-hook-form";
import { IGroupModel } from "../../models/IGroupModel";
import { HeatmapLayerFactory } from "@vgrid/react-leaflet-heatmap-layer";
import { GenerateMap } from "../MapComponents/GenerateMap";
import { MapControlBox } from "../MapComponents/MapControlBox";
import { SpeciesSearch } from "./SpeciesSearch";
import { Close } from "@mui/icons-material";
import { LocationFinder } from "../MapComponents/LocationFinder";
import MarkerClusterGroup from "react-leaflet-cluster";
import { CatchMarker } from "./CatchMarker";
import { ErrorComponent } from "../../common/ErrorComponent";
import { DatePicker } from "@mui/x-date-pickers";

enum MapType {
  heatmap = "Heatmap",
  markerCluster = "Marker cluster",
}
const today = new Date();
const HeatmapLayer = HeatmapLayerFactory<[number, number, number]>();
export const CatchesMap: React.FC<{
  latitude: number;
  longitude: number;
  currentMapZoom?: number;
  formMethods: UseFormReturn<SaveCatchInput>;
  setCurrentMapZoom: (z: number) => void;
  catchToEdit: boolean;
  group: IGroupModel;
}> = ({
  catchToEdit,
  group,
  latitude,
  longitude,
  setCurrentMapZoom,
  currentMapZoom,
  formMethods,
}) => {
  const { id: groupId } = useParams<{ id: string }>();
  const { permissionManager } = useCurrentPermissionManager();
  const { data: groupCatches, error: groupCatchesError } =
    useGetAllPartialCatchesForGroupQuery(groupId!);
  const [speciesFilter, setSpeciesFilter] = useState<string>();
  const [{ mapType, minWeight, endDate, startDate }, setMapFilters] = useState<{
    mapType?: MapType;
    minWeight: number;
    startDate?: Date;
    endDate?: Date;
  }>({
    mapType: MapType.markerCluster,
    minWeight: 0,
  });
  //Needs checking line 77 and 78 throwing
  const filteredCatches = groupCatches?.filter(
    (x) =>
      (!speciesFilter ||
        x.worldFish?.englishName?.toLocaleLowerCase() ===
          speciesFilter.toLocaleLowerCase()) &&
      x.weight >= minWeight &&
      (!startDate || new Date(x.caughtAt).getTime() >= startDate.getTime()) &&
      (!endDate || new Date(x.caughtAt).getTime() <= endDate.getTime())
  );
  return (
    <Grid
      container
      direction="column"
      alignItems="center"
      justifyContent="center"
      spacing={1}
    >
      <Grid item width="100%">
        <GenerateMap
          center={latitude && longitude ? [latitude, longitude] : undefined}
          zoom={currentMapZoom}
        >
          {!catchToEdit && groupCatches && groupCatches.length > 0 && (
            <MapControlBox>
              <Grid
                container
                justifyContent="center"
                alignItems="center"
                padding={1}
                spacing={1}
              >
                <Grid item width="80%">
                  <SpeciesSearch
                    setSpecies={setSpeciesFilter}
                    speciesString={speciesFilter}
                  />
                </Grid>
                <Grid item width="80%">
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
                        disabled={!!mapType}
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
                <Grid item width="80%">
                  <TextField
                    fullWidth
                    variant="outlined"
                    label="Minimum weight"
                    type="number"
                    InputLabelProps={{ shrink: true }}
                    InputProps={{
                      endAdornment: (
                        <InputAdornment position="end">lbs</InputAdornment>
                      ),
                    }}
                    onChange={(e) =>
                      setMapFilters({
                        mapType,
                        minWeight: Number(e.target.value),
                        endDate,
                        startDate,
                      })
                    }
                    value={minWeight}
                  />
                </Grid>
                <Grid item width={"80%"}>
                  <DatePicker
                    value={startDate}
                    onChange={(date) =>
                      setMapFilters({
                        mapType,
                        minWeight,
                        startDate: date ?? undefined,
                        endDate,
                      })
                    }
                    label="Start date"
                    maxDate={endDate ?? today}
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
                <Grid item width={"80%"}>
                  <DatePicker
                    value={endDate}
                    onChange={(date) =>
                      setMapFilters({
                        mapType,
                        minWeight,
                        startDate,
                        endDate: date ?? undefined,
                      })
                    }
                    label="End date"
                    minDate={startDate}
                    maxDate={today}
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
          )}
          {catchToEdit && (
            <LocationFinder
              lat={latitude && longitude ? latitude : undefined}
              lng={latitude && longitude ? longitude : undefined}
              setCurrentZoom={setCurrentMapZoom}
              setLatLng={({ lat, lng }) => {
                formMethods.setValue("latitude", lat.toString());
                formMethods.setValue("longitude", lng.toString());
              }}
            />
          )}
          {(group.catchesPublic ||
            permissionManager.Can(
              PermissionActions.Read,
              groupId!,
              PermissionFields.GroupCatch
            )) &&
            filteredCatches && (
              <>
                {mapType === MapType.markerCluster && (
                  <MarkerClusterGroup chunkedLoading>
                    {filteredCatches.map((pgc) => (
                      <CatchMarker
                        groupCatch={pgc}
                        groupId={groupId!}
                        useSnackBarOnSuccess
                      />
                    ))}
                  </MarkerClusterGroup>
                )}
                {mapType === MapType.heatmap && (
                  <HeatmapLayer
                    points={filteredCatches.map((x) => [
                      x.latitude,
                      x.longitude,
                      1,
                    ])}
                    fitBoundsOnLoad
                    fitBoundsOnUpdate
                    latitudeExtractor={(m) => m[0]}
                    longitudeExtractor={(m) => m[1]}
                    intensityExtractor={(m) => parseFloat(m[2].toString())}
                  />
                )}
                {!mapType && (
                  <>
                    {filteredCatches.map((pgc) => (
                      <CatchMarker
                        groupCatch={pgc}
                        groupId={groupId!}
                        useSnackBarOnSuccess
                      />
                    ))}
                  </>
                )}
              </>
            )}
        </GenerateMap>
      </Grid>
      {groupCatchesError && (
        <Grid item width="100%">
          <ErrorComponent error={groupCatchesError} />
        </Grid>
      )}
      {groupCatches && groupCatches.length === 0 && (
        <Grid item width="100%">
          <ErrorComponent
            error={new Error("No catches saved for this group")}
          />
        </Grid>
      )}
    </Grid>
  );
};
