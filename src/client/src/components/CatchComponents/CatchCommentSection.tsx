import { Grid } from "@mui/material";
import { useGetCatchCommentsQuery } from "./hooks/GetCatchComments";
import { CatchCommentForm } from "./CatchCommentForm";
import { ErrorComponent } from "../../common/ErrorComponent";
import { ApiException } from "../../common/ApiException";

export const CatchCommentSection: React.FC<{ groupCatchId: string }> = ({
  groupCatchId,
}) => {
  const { data: catchComments, error: catchCommentsError } =
    useGetCatchCommentsQuery(groupCatchId);
  return (
    <Grid container spacing={2}>
      {catchComments?.map((c) => (
        <Grid item width="100%">
          {c.comment}
        </Grid>
      ))}
      {catchCommentsError && (
        <Grid item width="100%">
          <ErrorComponent
            error={new ApiException(catchCommentsError.message)}
          />
        </Grid>
      )}
      <Grid item width="100%">
        <CatchCommentForm groupCatchId={groupCatchId} />
      </Grid>
    </Grid>
  );
};
