import { useForm } from "react-hook-form";
import { z } from "zod";
import { IGroupCatchCommentModel } from "../../models/IGroupCatchComment";
import { useCurrentUser } from "../../common/contexts/UserContext";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button, Grid } from "@mui/material";
import { useSaveCommentMutation } from "./hooks/SaveComment";
import { Mention, MentionsInput } from "react-mentions";
import { useSearchUsers } from "../../common/hooks/SearchUsers";
import { useEffect, useState } from "react";
import { IUserModel } from "../../models/IUserModel";
import { ErrorComponent } from "../../common/ErrorComponent";
import GroupCatchCommentTagsUtils from "./GroupCatchCommentTagsUtils";
const inputStyle = {
  control: {
    backgroundColor: "#fff",
    fontSize: 14,
    fontWeight: "normal",
  },

  "&multiLine": {
    control: {
      fontFamily: "monospace",
      minHeight: 63,
    },
    highlighter: {
      padding: 9,
      border: "1px solid transparent",
    },
    input: {
      padding: 9,
      border: "1px solid silver",
    },
  },

  "&singleLine": {
    display: "inline-block",
    width: 180,

    highlighter: {
      padding: 1,
      border: "2px inset transparent",
    },
    input: {
      padding: 1,
      border: "2px inset",
    },
  },

  suggestions: {
    list: {
      backgroundColor: "white",
      border: "1px solid rgba(0,0,0,0.15)",
      fontSize: 14,
    },
    item: {
      padding: "5px 15px",
      borderBottom: "1px solid rgba(0,0,0,0.15)",
      "&focused": {
        backgroundColor: "#cee4e5",
      },
    },
  },
};
const commentSchema = z.object({
  id: z.number().optional().nullable(),
  comment: z
    .string()
    .min(1, { message: "Please enter a comment before submitting" }),
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
  comment?: IGroupCatchCommentModel;
  groupCatchId: string;
}> = ({ comment, groupCatchId }) => {
  const { id: userId } = useCurrentUser();
  const {
    setValue,
    watch,
    reset: formReset,
    handleSubmit,
    formState: { errors: formErrors, isDirty },
  } = useForm<SaveCommentInput>({
    defaultValues: mapDefaultValues(userId!, groupCatchId, comment),
    resolver: zodResolver(commentSchema),
  });
  const [options, setOptions] = useState<Omit<IUserModel, "email">[]>([]);
  const {
    data: foundUsers,
    isLoading: foundUsersLoading,
    mutate: foundUsersMutate,
    reset: foundUsersReset,
  } = useSearchUsers();
  const {
    data: savedCommentId,
    isLoading: savedCommentIdLoading,
    mutate: saveComment,
  } = useSaveCommentMutation();
  const [personSearchTerm, setPersonSearchTerm] = useState("");
  useEffect(() => {
    if (savedCommentId) {
      formReset();
      foundUsersReset();
    }
  }, [savedCommentId, formReset, foundUsersReset]);
  useEffect(() => {
    if (personSearchTerm) {
      foundUsersReset();
      foundUsersMutate({
        searchTerm: personSearchTerm,
      });
    }
  }, [personSearchTerm, foundUsersMutate, foundUsersReset]);
  useEffect(() => {
    if (foundUsers) {
      setOptions(foundUsers);
    }
  }, [foundUsers]);
  const { comment: formComment } = watch();
  return (
    <form
      id="catchCommentForm"
      onSubmit={handleSubmit((v) => {
        saveComment({
          ...v,
          comment:
            GroupCatchCommentTagsUtils.ConvertRawTagStringToFormattedTagString(
              v.comment
            ),
        });
      })}
    >
      <Grid
        container
        spacing={1}
        width={"100%"}
        justifyContent="center"
        alignItems="center"
      >
        <Grid item width={"100%"}>
          <MentionsInput
            value={formComment}
            placeholder="Type a comment..."
            style={inputStyle}
            allowSuggestionsAboveCursor
            allowSpaceInQuery
            singleLine={false}
            onChange={(e) => {
              const totalInput = e.target.value;
              if (totalInput?.length < 1) {
                formReset();
                return;
              }
              setValue("comment", totalInput, { shouldDirty: true });
              const mentionRegex = /@(\w+)(?=\s|$)/g;
              const match = mentionRegex.exec(totalInput);
              if (match) {
                const mentionIdentifier = match[1];
                setPersonSearchTerm(`${mentionIdentifier}`);
              } else {
                setPersonSearchTerm("");
              }
            }}
          >
            <Mention
              trigger={"@"}
              isLoading={foundUsersLoading}
              appendSpaceOnAdd
              displayTransform={(_, display) => `@${display}`}
              style={{ backgroundColor: "#EBEBEB" }}
              data={options.map((x) => ({ id: x.id!, display: x.username }))}
            />
          </MentionsInput>
        </Grid>
        {formErrors && Object.values(formErrors).some((x) => !!x) && (
          <Grid item width={"100%"}>
            <ErrorComponent error={formErrors} />
          </Grid>
        )}
        <Grid item width={"100%"} display={"flex"} justifyContent={"flex-end"}>
          <Button
            disabled={!isDirty || savedCommentIdLoading}
            variant="contained"
            type="submit"
            color="primary"
          >
            Save comment
          </Button>
        </Grid>
      </Grid>
    </form>
  );
};
