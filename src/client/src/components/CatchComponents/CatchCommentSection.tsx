import { Divider, Grid, Paper } from "@mui/material";
import { useGetCatchCommentsQuery } from "./hooks/GetCatchComments";
import { CatchCommentForm } from "./CatchCommentForm";
import { ErrorComponent } from "../../common/ErrorComponent";
import { ApiException } from "../../common/ApiException";
import { GroupCatchCommentItem } from "./GroupCatchCommentItem";
import { useDeleteCommentMutation } from "./hooks/DeleteComment";

export const CatchCommentSection: React.FC<{
  groupCatchId: string;
  groupId: string;
}> = ({ groupCatchId, groupId }) => {
  const { data: catchComments, error: catchCommentsError } =
    useGetCatchCommentsQuery(groupCatchId);
  const { isLoading: isDeleting, mutate: deleteComment } =
    useDeleteCommentMutation();
  return (
    <Paper elevation={2}>
      <Grid container overflow={"auto"}>
        {catchComments && catchComments.length >= 1 && (
          <Grid item width="100%" mt={2}>
            <Grid
              container
              maxHeight={"40vh"}
              overflow={"auto"}
              spacing={3.5}
              padding={1.7}
            >
              {catchComments?.map((c) => (
                <Grid item width="100%">
                  <GroupCatchCommentItem
                    groupId={groupId}
                    groupCatchComment={c}
                    onDelete={{
                      deleteFunc: deleteComment,
                      deleteLoading: isDeleting,
                    }}
                  />
                </Grid>
              ))}
            </Grid>
          </Grid>
        )}
        <Grid item width="100%">
          <Grid container spacing={2} padding={1}>
            {catchCommentsError && (
              <Grid item width="100%">
                <ErrorComponent
                  error={new ApiException(catchCommentsError.message)}
                />
              </Grid>
            )}
            {catchComments && catchComments.length >= 1 && (
              <Grid item width="100%">
                <Divider />
              </Grid>
            )}
            <Grid item width="100%">
              <CatchCommentForm groupCatchId={groupCatchId} />
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Paper>
  );
};
