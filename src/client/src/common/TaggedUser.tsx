import { Chip, Grid, Link } from "@mui/material";
import Avatar from "react-avatar";
import { Link as RouterLink } from "react-router-dom";

export const TaggedUser: React.FC<{
  chip?: {
    color?: "primary" | "secondary" | "success" | "error" | "info" | "default";
  };
  user: { id: string; username: string };
  avatarSize?: string;
}> = ({ chip, user: { id: userId, username }, avatarSize = "30" }) => {
  if (chip) {
    return (
      <Chip
        color={chip.color}
        label={<TaggedUser user={{ id: userId, username }} avatarSize="24" />}
      />
    );
  }
  return (
    <Grid
      direction={"row"}
      spacing={0.5}
      padding={0.12}
      container
      justifyContent={"center"}
      alignItems={"center"}
    >
      <Grid item>
        <Avatar email={username} size={avatarSize} round={"4px"} />
      </Grid>
      <Grid item>
        <Link to={`/account/${userId}`} component={RouterLink}>
          {username}
        </Link>
      </Grid>
    </Grid>
  );
};
