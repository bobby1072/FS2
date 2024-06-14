import { Grid } from "@mui/material";
import { useParams } from "react-router-dom";
import { useState } from "react";
import { HeatmapLayerFactory } from "@vgrid/react-leaflet-heatmap-layer";
import { GenerateMap } from "../MapComponents/GenerateMap";
import { LocationFinder } from "../MapComponents/LocationFinder";
import MarkerClusterGroup from "react-leaflet-cluster";
import { CatchMarkerForPartialCatch } from "./CatchMarker";
import { getEarliestAndLatestDate } from "../../utils/DateTime";
import { RuntimePartialGroupCatchModel } from "../../models/IGroupCatchModel";
import { CatchesMapSetFilterBox, MapType } from "./CatchesMapFilterBox";

const HeatmapLayer = HeatmapLayerFactory<[number, number, number]>();

export const GroupCatchesMap: React.FC<{
  latitude?: number;
  longitude?: number;
  catchToEdit?: boolean;
  setLat?: (lat: string) => void;
  setLng?: (lng: string) => void;
  groupCatches: RuntimePartialGroupCatchModel[];
  zoom?: number;
}> = ({
  catchToEdit = false,
  latitude,
  zoom = 1,
  longitude,
  groupCatches,
  setLat,
  setLng,
}) => {
  const { id: groupId } = useParams<{ id: string }>();
  const [speciesFilter, setSpeciesFilter] = useState<string>();
  const [mapFilters, setMapFilters] = useState<{
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
  const { minLength, minWeight, endDate, mapType, startDate } = mapFilters;
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
          zoom={zoom}
          center={latitude && longitude ? [latitude, longitude] : undefined}
        >
          {!catchToEdit && groupCatches && groupCatches.length > 0 && (
            <CatchesMapSetFilterBox
              mapFilters={mapFilters}
              setMapFilters={setMapFilters}
              setSpeciesFilter={setSpeciesFilter}
              speciesFilter={speciesFilter}
              earliestDate={earliestDate}
              latestDate={latestDate}
            />
          )}
          {catchToEdit && (
            <LocationFinder
              lat={latitude && longitude ? latitude : undefined}
              lng={latitude && longitude ? longitude : undefined}
              setLatLng={({ lat, lng }) => {
                setLat?.(lat.toString());
                setLng?.(lng.toString());
              }}
            />
          )}
          {filteredCatches && (
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
