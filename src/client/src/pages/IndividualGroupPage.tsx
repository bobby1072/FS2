import { useParams } from "react-router-dom";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { PageBase } from "../common/PageBase";
import { useGetFullGroup } from "../components/GroupComponents/hooks/GetFullGroup";
import { Box, Button, Grid, Paper, Typography } from "@mui/material";
import { Loading } from "../common/Loading";
import { GroupMembersDataTable } from "../components/GroupComponents/GroupMembersDataTable";
import { GroupPositionDataTable } from "../components/GroupComponents/GroupPositionDataTable";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionSet,
} from "../common/contexts/AbilitiesContext";
import { ErrorComponent } from "../common/ErrorComponent";
import { useState } from "react";
import { IGroupCatchModel } from "../models/IGroupCatchModel";
import { SaveGroupCatchForm } from "../components/CatchComponents/SaveGroupCatchForm";

export const IndividualGroupPage: React.FC = () => {
  const { id: groupId } = useParams<{ id: string }>();
  const { data: mainGroup, error, isLoading } = useGetFullGroup(groupId);
  const { permissionManager } = useCurrentPermissionSet();
  const [catchToEdit, setCatchToEdit] = useState<IGroupCatchModel | boolean>();
  if (error) return <ErrorComponent fullScreen error={error} />;
  else if (!mainGroup || isLoading) return <Loading fullScreen />;
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
                  Clear new catch
                </Button>
              )}
            </Grid>
          )}
          {catchToEdit && (
            <Grid item width="100%">
              <Paper elevation={2}>
                <SaveGroupCatchForm
                  closeForm={() => setCatchToEdit(false)}
                  useSnackBarOnSuccess
                  groupId={groupId!}
                  groupCatch={
                    typeof catchToEdit !== "boolean" ? catchToEdit : undefined
                  }
                />
              </Paper>
            </Grid>
          )}
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
