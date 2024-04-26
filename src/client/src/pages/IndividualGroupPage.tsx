import { useParams } from "react-router-dom";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { PageBase } from "../common/PageBase";
import { useGetFullGroup } from "../components/GroupComponents/hooks/GetFullGroup";
import {
  Accordion,
  AccordionDetails,
  Box,
  Button,
  Grid,
  Paper,
  Typography,
} from "@mui/material";
import { Loading } from "../common/Loading";
import { GroupMembersDataTable } from "../components/GroupComponents/GroupMembersDataTable";
import { GroupPositionDataTable } from "../components/GroupComponents/GroupPositionDataTable";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionSet,
} from "../common/contexts/AbilitiesContext";
import { ErrorComponent } from "../common/ErrorComponent";
import { useEffect, useState } from "react";
import { IGroupCatchModel } from "../models/IGroupCatchModel";
import {
  SaveCatchInput,
  SaveGroupCatchForm,
  formSchema,
  mapDefaultValues,
} from "../components/CatchComponents/SaveGroupCatchForm";
import { useGetAllPartialCatchesForGroupQuery } from "../components/CatchComponents/hooks/GetAllPartialCatchesForGroup";
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { LocationFinder } from "../components/MapComponents/LocationFinder";
import { GenerateMap } from "../components/MapComponents/GenerateMap";
import { IGroupModel } from "../models/IGroupModel";
import { CatchMarker } from "../components/CatchComponents/CatchMarker";

export const IndividualGroupPage: React.FC = () => {
  const { id: groupId } = useParams<{ id: string }>();
  const { data: mainGroup, error, isLoading } = useGetFullGroup(groupId);
  const { permissionManager } = useCurrentPermissionSet();
  if (
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
  else if (!mainGroup || isLoading) return <Loading fullScreen />;
  return <IndividualGroupPageInner group={mainGroup} />;
};

const IndividualGroupPageInner: React.FC<{
  group: Omit<IGroupModel, "members">;
}> = ({ group: mainGroup }) => {
  const { id: groupId } = useParams<{ id: string }>();
  const { permissionManager } = useCurrentPermissionSet();
  const { data: groupCatches, error: groupCatchesError } =
    useGetAllPartialCatchesForGroupQuery(groupId!);
  const [currentMapZoom, setCurrentMapZoom] = useState<number>();
  const [catchToEdit, setCatchToEdit] = useState<IGroupCatchModel | boolean>();
  const formMethods = useForm<SaveCatchInput>({
    defaultValues: mapDefaultValues(
      groupId!,
      catchToEdit && typeof catchToEdit !== "boolean" ? catchToEdit : undefined
    ),
    resolver: zodResolver(formSchema),
  });
  useEffect(() => {
    formMethods.reset();
  }, [catchToEdit, formMethods]);
  const { latitude, longitude } = formMethods.watch();
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
  return (
    <PageBase>
      <AppAndDraw>
        <Grid container justifyContent="center" alignItems="center" spacing={2}>
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
                  <FormProvider {...formMethods}>
                    <SaveGroupCatchForm
                      closeForm={() => setCatchToEdit(false)}
                      useSnackBarOnSuccess
                      showMapInfoMessage
                      groupCatch={
                        typeof catchToEdit !== "boolean"
                          ? catchToEdit
                          : undefined
                      }
                    />
                  </FormProvider>
                </AccordionDetails>
              </Accordion>
            </Grid>
          )}
          {groupCatchesError && (
            <Grid item width="100%">
              <ErrorComponent error={groupCatchesError} />
            </Grid>
          )}
          <Grid item width="100%">
            <GenerateMap
              center={latitude && longitude ? [latitude, longitude] : undefined}
              zoom={currentMapZoom}
            >
              {catchToEdit && (
                <LocationFinder
                  lat={latitude ? latitude : undefined}
                  lng={latitude ? longitude : undefined}
                  setCurrentZoom={setCurrentMapZoom}
                  setLatLng={({ lat, lng }) => {
                    formMethods.setValue("latitude", lat);
                    formMethods.setValue("longitude", lng);
                  }}
                />
              )}
              {groupCatches && (
                <>
                  {groupCatches.map((gc) => (
                    <CatchMarker groupCatch={gc} />
                  ))}
                </>
              )}
            </GenerateMap>
          </Grid>
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
