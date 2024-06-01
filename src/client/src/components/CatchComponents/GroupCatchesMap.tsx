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
import { CatchMarkerForPartialCatch } from "./CatchMarker";
import { DatePicker } from "@mui/x-date-pickers";
import { getEarliestAndLatestDate } from "../../utils/DateTime";
import { RuntimePartialGroupCatchModel } from "../../models/IGroupCatchModel";

enum MapType {
  heatmap = "Heatmap",
  markerCluster = "Marker cluster",
}
const HeatmapLayer = HeatmapLayerFactory<[number, number, number]>();

export const GroupCatchesMap: React.FC<{
  latitude: number;
  longitude: number;
  currentMapZoom?: number;
  formMethods: UseFormReturn<SaveCatchInput>;
  setCurrentMapZoom: (z: number) => void;
  catchToEdit: boolean;
  group: IGroupModel;
  groupCatches: RuntimePartialGroupCatchModel[]
}> = ({
  catchToEdit,
  group,
  latitude,
  longitude,
  setCurrentMapZoom,
  currentMapZoom,
  formMethods,
  groupCatches
}) => {
    const { id: groupId } = useParams<{ id: string }>();
    const { permissionManager } = useCurrentPermissionManager();
    const [speciesFilter, setSpeciesFilter] = useState<string>();
    const [{ mapType, minWeight, endDate, startDate, minLength }, setMapFilters] =
      useState<{
        mapType?: MapType;
        minWeight: number;
        minLength: number;
        startDate?: Date;
        endDate?: Date;
      }>({
        mapType: MapType.markerCluster,
        minWeight: 0,
        minLength: 0,
      });

    const filteredCatches = groupCatches?.filter(
      (x) =>
        (!speciesFilter ||
          x.worldFish?.englishName?.toLocaleLowerCase() ===
          speciesFilter.toLocaleLowerCase()) &&
        x.weight >= minWeight &&
        (!startDate || x.caughtAt.getTime() >= startDate.getTime()) &&
        (!endDate || x.caughtAt.getTime() <= endDate.getTime()) &&
        x.length >= minLength
    );
    const { earliestDate, latestDate } = getEarliestAndLatestDate(
      groupCatches?.map((x) => x.caughtAt) ?? []
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
                        endAdornment: (
                          <InputAdornment position="end">cm</InputAdornment>
                        ),
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
                        <CatchMarkerForPartialCatch
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
                        <CatchMarkerForPartialCatch
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

      </Grid>
    );
  };
