import { useForm } from "react-hook-form";
import { z } from "zod";
import { IGroupCatchCommentModel } from "../../models/IGroupCatchComment";
import { useCurrentUser } from "../../common/contexts/UserContext";
import { zodResolver } from "@hookform/resolvers/zod";
import { Grid } from "@mui/material";
import { useSaveCommentMutation } from "./hooks/SaveComment";

const commentSchema = z.object({
  id: z.number().optional().nullable(),
  comment: z.string().min(1),
  groupCatchId: z.string().uuid(),
  userId: z.string().uuid(),
  createdAt: z.date().optional().nullable(),
});

export type SaveCommentInput = z.infer<typeof commentSchema>;
const mapDefaultValues = (
  userId: string,
  groupCatchId: string,
  comment?: IGroupCatchCommentModel
): Partial<SaveCommentInput> => {
  if (!comment) return { userId, groupCatchId };
  return {
    id: comment.id,
    comment: comment.comment,
    groupCatchId: comment.groupCatchId,
    userId: comment.userId,
    createdAt: comment.createdAt,
  };
};
export const CatchCommentForm: React.FC<{
  comment: IGroupCatchCommentModel;
  groupCatchId: string;
}> = ({ comment, groupCatchId }) => {
  const { id: userId } = useCurrentUser();
  const {
    setValue,
    watch,
    reset,
    handleSubmit,
    formState: { errors },
  } = useForm<SaveCommentInput>({
    defaultValues: mapDefaultValues(userId!, groupCatchId, comment),
    resolver: zodResolver(commentSchema),
  });
  const { data, isLoading, error } = useSaveCommentMutation();
  return (
    <form>
      <Grid container></Grid>
    </form>
  );
};
