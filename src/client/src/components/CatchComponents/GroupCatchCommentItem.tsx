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
import { useDeleteCommentMutation } from "./hooks/DeleteComment";
import DeleteIcon from "@mui/icons-material/Delete";
import { useCurrentUser } from "../../common/contexts/UserContext";
import { ErrorComponent } from "../../common/ErrorComponent";
import GroupCatchCommentTagsUtils from "./GroupCatchCommentTagsUtils";

export const GroupCatchCommentItem: React.FC<{
  groupCatchComment: IGroupCatchCommentModel;
  groupId: string;
}> = ({ groupCatchComment, groupId }) => {
  const { permissionManager } = useCurrentPermissionManager();
  const {
    error: deleteError,
    isLoading: isDeleting,
    mutate: deleteComment,
  } = useDeleteCommentMutation();
  const { id: currentUserId } = useCurrentUser();
  const canDeleteComment =
    permissionManager.Can(
      PermissionActions.Manage,
      groupId,
      PermissionFields.GroupCatch
    ) || groupCatchComment.userId === currentUserId;
  return (
    <Paper elevation={2}>
      <Grid
        container
        spacing={3}
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
              label={new Date(groupCatchComment.createdAt)
                .toDateString()
                .substring(0, 10)}
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
                disabled={isDeleting}
                onClick={() => {
                  deleteComment(groupCatchComment.id!);
                }}
              >
                <DeleteIcon />
              </IconButton>
            </Tooltip>
          </Grid>
        )}
        {deleteError && (
          <Grid item width={"100%"}>
            <ErrorComponent error={deleteError} />
          </Grid>
        )}
      </Grid>
    </Paper>
  );
};
