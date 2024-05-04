import { useParams } from "react-router-dom";
import { useGetFullCatchQuery } from "../components/CatchComponents/hooks/GetFullCatch";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../common/contexts/AbilitiesContext";
import { ErrorComponent } from "../common/ErrorComponent";
import { Loading } from "../common/Loading";

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
  return null;
};
