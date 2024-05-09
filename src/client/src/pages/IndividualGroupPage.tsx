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
import { FormProvider, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { IGroupModel } from "../models/IGroupModel";
import { CatchesMap } from "../components/CatchComponents/CatchMap";

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
                <Accordion expanded>
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
