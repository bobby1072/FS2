import { useParams } from "react-router-dom";
import { useGetFullCatchQuery } from "../components/CatchComponents/hooks/GetFullCatch";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../common/contexts/AbilitiesContext";
import { ErrorComponent } from "../common/ErrorComponent";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { Grid, Paper, Typography } from "@mui/material";
import { getPrettyWorldFishName } from "../common/GetPrettyWorldFish";

export const IndividualCatchPage: React.FC = () => {
  const { id: catchId } = useParams<{ id: string }>();
  const {
    data: fullCatch,
    isLoading: catchLoading,
    error: catchError,
  } = useGetFullCatchQuery(catchId);
  const { permissionManager } = useCurrentPermissionManager();
  if (catchLoading) return <Loading fullScreen />;
  else if (
    fullCatch &&
    !fullCatch.group?.catchesPublic &&
    !permissionManager.Can(
      PermissionActions.Read,
      fullCatch.groupId,
      PermissionFields.GroupCatch
    )
  ) {
    return (
      <ErrorComponent
        fullScreen
        error={new Error("You do not have permissions to view this catch")}
      />
    );
  } else if (catchError)
    return <ErrorComponent fullScreen error={catchError} />;
  else if (!fullCatch) return <ErrorComponent fullScreen />;
  return (
    <PageBase>
      <AppAndDraw>
        <Grid container justifyContent="center" alignItems="center" spacing={2}>
          <Grid item width={"80%"}>
            <Paper elevation={2}>
              <Grid
                container
                justifyContent="center"
                alignItems="center"
                spacing={2}
                padding={4}
              >
                <Grid item width={"100%"}>
                  <Typography
                    variant="h3"
                    textAlign="center"
                    fontSize={50}
                    overflow="auto"
                  >
                    {fullCatch.worldFish
                      ? getPrettyWorldFishName(fullCatch.worldFish)
                      : fullCatch.species}
                  </Typography>
                </Grid>
                <Grid item width={"100%"}>
                  <Typography
                    variant="body2"
                    textAlign="center"
                    fontSize={15}
                    overflow="auto"
                  >
                    Caught by {fullCatch.user!.username}
                  </Typography>
                </Grid>
              </Grid>
            </Paper>
          </Grid>
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
