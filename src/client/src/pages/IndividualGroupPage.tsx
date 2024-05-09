import { useParams } from "react-router-dom";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { PageBase } from "../common/PageBase";
import { useGetFullGroup } from "../components/GroupComponents/hooks/GetFullGroup";
import {
  Accordion,
  AccordionDetails,
  Box,
  Button,
  FormControl,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Slider,
  TextField,
  Typography,
} from "@mui/material";
import { Loading } from "../common/Loading";
import { GroupMembersDataTable } from "../components/GroupComponents/GroupMembersDataTable";
import { GroupPositionDataTable } from "../components/GroupComponents/GroupPositionDataTable";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../common/contexts/AbilitiesContext";
import { ErrorComponent } from "../common/ErrorComponent";
import { useEffect, useState } from "react";
import {
  SaveCatchInput,
  SaveGroupCatchForm,
  formSchema,
  mapDefaultValuesToSaveCatchInput,
} from "../components/CatchComponents/SaveGroupCatchForm";
import { Close } from "@mui/icons-material";
import { useGetAllPartialCatchesForGroupQuery } from "../components/CatchComponents/hooks/GetAllPartialCatchesForGroup";
import { FormProvider, UseFormReturn, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { LocationFinder } from "../components/MapComponents/LocationFinder";
import { GenerateMap } from "../components/MapComponents/GenerateMap";
import { IGroupModel } from "../models/IGroupModel";
import { CatchMarker } from "../components/CatchComponents/CatchMarker";
import MarkerClusterGroup from "react-leaflet-cluster";
import { MapControlBox } from "../components/MapComponents/MapControlBox";
import { SpeciesSearch } from "../components/CatchComponents/SpeciesSearch";
import { HeatmapLayerFactory } from "@vgrid/react-leaflet-heatmap-layer";

export const IndividualGroupPage: React.FC = () => {
  const { id: groupId } = useParams<{ id: string }>();
  const { data: mainGroup, error, isLoading } = useGetFullGroup(groupId);
  const { permissionManager } = useCurrentPermissionManager();
  if (isLoading) return <Loading fullScreen />;
  else if (
    mainGroup &&
    !mainGroup?.public &&
    !permissionManager.Can(PermissionActions.BelongsTo, groupId!)
  )
    return (
      <ErrorComponent
        fullScreen
        error={new Error("You do not have permission to view this group.")}
      />
    );
  else if (error) return <ErrorComponent fullScreen error={error} />;
  else if (!mainGroup) return <ErrorComponent fullScreen />;
  return <IndividualGroupPageInner group={mainGroup} />;
};

const IndividualGroupPageInner: React.FC<{
  group: Omit<IGroupModel, "members">;
}> = ({ group: mainGroup }) => {
  const { id: groupId } = useParams<{ id: string }>();
  const { permissionManager } = useCurrentPermissionManager();
  const [currentMapZoom, setCurrentMapZoom] = useState<number>();
  const {
    name: groupName,
    emblem: groupEmblem,
    description: groupDescription,
    leader: groupLeader,
    positions: allPositions,
  } = mainGroup;
  const canReadMembers = permissionManager.Can(
    PermissionActions.Read,
    groupId!,
    PermissionFields.GroupMember
  );
  const [catchToEdit, setCatchToEdit] = useState<boolean>();
  const formMethods = useForm<SaveCatchInput>({
    defaultValues: mapDefaultValuesToSaveCatchInput(
      groupId!,
      catchToEdit && typeof catchToEdit !== "boolean" ? catchToEdit : undefined
    ),
    resolver: zodResolver(formSchema),
  });
  const { latitude, longitude } = formMethods.watch();
  useEffect(() => {
    formMethods.reset();
  }, [catchToEdit, formMethods]);
  return (
    <PageBase>
      <AppAndDraw>
        <FormProvider {...formMethods}>
          <Grid
            container
            justifyContent="center"
            alignItems="center"
            spacing={2}
          >
            <Grid item width="80%">
              <Paper elevation={2}>
                <Grid
                  container
                  direction={"column"}
                  justifyContent="center"
                  alignItems="center"
                  padding={4}
                  spacing={2}
                >
                  <Grid item width="100%">
                    <Typography
                      variant="h3"
                      textAlign="center"
                      fontSize={50}
                      overflow="auto"
                    >
                      {groupName}
                    </Typography>
                  </Grid>
                  {groupEmblem && (
                    <Grid item>
                      <Box
                        component="img"
                        sx={{
                          border: "0.1px solid #999999",
                          maxHeight: "80vh",
                          width: "100%",
                        }}
                        src={`data:image/jpeg;base64,${groupEmblem}`}
                        alt={`emblem: ${groupId}`}
                      />
                    </Grid>
                  )}
                  <Grid item>
                    <Typography
                      variant="subtitle2"
                      textAlign="center"
                      fontSize={17}
                    >
                      <strong>Id: </strong>
                      {groupId}
                    </Typography>
                  </Grid>
                  {groupDescription && (
                    <Grid item>
                      <Typography
                        variant="subtitle2"
                        textAlign="center"
                        fontSize={20}
                      >
                        <strong>Description: </strong>
                        {groupDescription}
                      </Typography>
                    </Grid>
                  )}
                </Grid>
              </Paper>
            </Grid>
            {canReadMembers && (
              <Grid item width="50%">
                <GroupMembersDataTable
                  leader={(groupLeader as any) ?? undefined}
                  positions={allPositions ?? []}
                  groupId={mainGroup.id!}
                />
              </Grid>
            )}
            <Grid item width={canReadMembers ? "50%" : "100%"}>
              <GroupPositionDataTable
                positions={allPositions ?? []}
                groupId={mainGroup.id!}
              />
            </Grid>
            {permissionManager.Can(PermissionActions.BelongsTo, groupId!) && (
              <Grid
                item
                width="100%"
                sx={{ display: "flex", justifyContent: "flex-end", mb: 1 }}
              >
                {!catchToEdit ? (
                  <Button
                    onClick={() => setCatchToEdit(true)}
                    variant="contained"
                  >
                    Create new catch
                  </Button>
                ) : (
                  <Button
                    variant="outlined"
                    onClick={() => setCatchToEdit(false)}
                  >
                    Cancel
                  </Button>
                )}
              </Grid>
            )}
            {catchToEdit && (
              <Grid item width="100%">
                <Accordion>
                  <AccordionDetails>
                    <SaveGroupCatchForm
                      closeForm={() => setCatchToEdit(false)}
                      useSnackBarOnSuccess
                      showMapInfoMessage
                    />
                  </AccordionDetails>
                </Accordion>
              </Grid>
            )}
            {(mainGroup.catchesPublic ||
              permissionManager.Can(
                PermissionActions.Read,
                groupId!,
                PermissionFields.GroupCatch
              ) ||
              permissionManager.Can(PermissionActions.BelongsTo, groupId!)) && (
              <Grid item width="100%">
                <CatchesMap
                  catchToEdit={!!catchToEdit}
                  latitude={Number(latitude)}
                  group={mainGroup}
                  longitude={Number(longitude)}
                  setCurrentMapZoom={setCurrentMapZoom}
                  currentMapZoom={currentMapZoom}
                  formMethods={formMethods}
                />
              </Grid>
            )}
          </Grid>
        </FormProvider>
      </AppAndDraw>
    </PageBase>
  );
};
enum MapType {
  heatmap = "Heatmap",
  markerCluster = "Marker cluster",
}
const HeatmapLayer = HeatmapLayerFactory<[number, number, number]>();
const CatchesMap: React.FC<{
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
  const [mapType, setMapType] = useState<MapType | undefined>(
    MapType.markerCluster
  );
  const filteredCatches = groupCatches?.filter(
    (x) =>
      !speciesFilter ||
      x.worldFish?.englishName?.toLocaleLowerCase() ===
        speciesFilter.toLocaleLowerCase()
  );
  const maxWeight = groupCatches
    ? Math.max(...groupCatches.map((x) => x.weight))
    : 0;

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
          <MapControlBox>
            <Grid
              container
              justifyContent="center"
              alignItems="center"
              padding={0.5}
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
                          onClick={() => setMapType(undefined)}
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
                              onClick={() => setMapType(undefined)}
                            >
                              <Close fontSize="inherit" />
                            </IconButton>
                          </InputAdornment>
                        )
                      }
                      onChange={(v) => {
                        setMapType(v.target.value as MapType);
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
                <Slider
                  defaultValue={0}
                  valueLabelDisplay="auto"
                  max={maxWeight}
                  min={0}
                  title="Minimum weight"
                />
              </Grid>
            </Grid>
          </MapControlBox>
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
