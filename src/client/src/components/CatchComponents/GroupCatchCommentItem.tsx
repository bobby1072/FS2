import {
  Chip,
  Grid,
  IconButton,
  Paper,
  Tooltip,
  Typography,
} from "@mui/material";
import { IGroupCatchCommentModel } from "../../models/IGroupCatchComment";
import { TaggedUser } from "../../common/TaggedUser";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../../common/contexts/AbilitiesContext";
import DeleteIcon from "@mui/icons-material/Delete";
import { useCurrentUser } from "../../common/contexts/UserContext";
import GroupCatchCommentTagsUtils from "./GroupCatchCommentTagsUtils";
import { formattedDate } from "../../utils/DateTime";

export const GroupCatchCommentItem: React.FC<{
  groupCatchComment: IGroupCatchCommentModel;
  groupId: string;
  onDelete?: {
    deleteFunc: (id: number) => void;
    deleteLoading: boolean;
  };
}> = ({ groupCatchComment, groupId, onDelete }) => {
  const { permissionManager } = useCurrentPermissionManager();
  const { id: currentUserId } = useCurrentUser();
  const canDeleteComment =
    onDelete &&
    (permissionManager.Can(
      PermissionActions.Manage,
      groupId,
      PermissionFields.GroupCatch
    ) ||
      groupCatchComment.userId === currentUserId);
  return (
    <Paper elevation={2}>
      <Grid
        container
        spacing={1.3}
        padding={1.4}
        justifyContent={"center"}
        alignItems={"center"}
      >
        <Grid item width={"20%"} display={"flex"} justifyContent={"flex-start"}>
          <TaggedUser
            chip={{ color: "default" }}
            user={{
              id: groupCatchComment.userId!,
              username: groupCatchComment.user!.username,
            }}
          />
        </Grid>
        <Grid item width={"80%"} display={"flex"} justifyContent={"flex-end"}>
          <Typography variant="body1" fontSize={12}>
            Posted:{" "}
            <Chip
              color="default"
              label={formattedDate(
                new Date(groupCatchComment.createdAt),
                "dd/mm/yyyy"
              )}
            />
          </Typography>
        </Grid>
        <Grid item width={canDeleteComment ? "90%" : "100%"}>
          <Typography variant="body1">
            {GroupCatchCommentTagsUtils.RenderCommentString(groupCatchComment)}
          </Typography>
        </Grid>
        {canDeleteComment && (
          <Grid item width={"10%"} display={"flex"} justifyContent={"flex-end"}>
            <Tooltip title="Delete Comment">
              <IconButton
                size="small"
                color="primary"
                disabled={onDelete.deleteLoading}
                onClick={() => {
                  onDelete.deleteFunc(groupCatchComment.id!);
                }}
              >
                <DeleteIcon />
              </IconButton>
            </Tooltip>
          </Grid>
        )}
      </Grid>
    </Paper>
  );
};
