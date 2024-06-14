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
import { Grid } from "@mui/material";
import { GenerateMap } from "../components/MapComponents/GenerateMap";
import { SimpleLongLatCatchMarkerWithPopup } from "../components/CatchComponents/CatchMarker";
import { CatchPaperForm } from "../components/CatchComponents/CatchPaperForm";
import { CatchCommentSection } from "../components/CatchComponents/CatchCommentSection";

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
  const catchPosition = fullCatch.GetPosition();
  return (
    <PageBase>
      <AppAndDraw>
        <Grid container justifyContent="center" alignItems="center" spacing={4}>
          <Grid item width={"80%"}>
            <CatchPaperForm fullCatch={fullCatch} />
          </Grid>
          <Grid item width={"100%"}>
            <GenerateMap center={catchPosition} zoom={9}>
              <SimpleLongLatCatchMarkerWithPopup position={catchPosition} />
            </GenerateMap>
          </Grid>
          {permissionManager.Can(
            PermissionActions.Read,
            fullCatch.groupId,
            PermissionFields.GroupCatch
          ) && (
            <Grid item width={"100%"}>
              <CatchCommentSection
                useSnackBar
                groupId={fullCatch.groupId}
                groupCatchId={catchId!}
              />
            </Grid>
          )}
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
