import { Chip, Link } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { IGroupCatchCommentModel } from "../../models/IGroupCatchComment";

export default abstract class GroupCatchCommentTagsUtils {
  public static readonly JustGuidRegex =
    /([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})/;
  public static readonly CommentTagsRegexPattern =
    /@([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})/;
  public static ConvertRawTagStringToFormattedTagString(input: string) {
    const regex = /\[([^\]]+)\]\(([^)]+)\)/g;
    const convertedString = input.replace(regex, (match, name, id) => {
      return id;
    });
    return convertedString;
  }
  public static RenderCommentString(comment: IGroupCatchCommentModel) {
    const foundMatches = this.CommentTagsRegexPattern.exec(
      comment.comment
    )?.filter((x) => x.includes("@"));
    if (!foundMatches || foundMatches.length === 0) return comment.comment;
    const newTempComment = `${comment.comment}`;
    return (
      <>
        {newTempComment.split(this.CommentTagsRegexPattern).map((part) => {
          if (!this.JustGuidRegex.test(part)) return part;
          else {
            const personNeededToPlatedIn = comment.taggedUsers?.find(
              (x) => x.userId === part
            );
            if (!personNeededToPlatedIn) return part;
            return (
              <Chip
                key={personNeededToPlatedIn.id}
                size={"small"}
                variant={"filled"}
                label={
                  <Link
                    to={`/viewperson/${personNeededToPlatedIn.id}`}
                    component={RouterLink}
                  >
                    @{personNeededToPlatedIn.user?.username}
                  </Link>
                }
              />
            );
          }
        })}
      </>
    );
  }
}
