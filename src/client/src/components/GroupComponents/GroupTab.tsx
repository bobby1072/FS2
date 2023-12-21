import { Grid, Paper, Typography } from "@mui/material";
import { GroupModel } from "../../models/GroupModel";
import Avatar from "react-avatar";

export const GroupTab: React.FC<{ group: GroupModel }> = ({ group }) => {
  const initials = group.name
    ?.split(" ")
    .map((x) => x[0])
    .join("");
  return (
    <Paper elevation={2}>
      <Grid
        container
        width="100%"
        direction="column"
        justifyContent="center"
        alignItems="center"
        textAlign={"center"}
        spacing={2}
        padding={2}
      >
        <Grid item width="100%">
          {group.emblem ? (
            <img src={group.emblem.toString()} alt={group.id ?? ""} />
          ) : (
            <Avatar initials={initials} />
          )}
        </Grid>
        <Grid item width="100%">
          <Typography variant="h4" fontSize={16}>
            {group.id}
          </Typography>
        </Grid>
        <Grid item width="100%">
          <Typography variant="h2" fontSize={35}>
            {group.name}
          </Typography>
        </Grid>
        <Grid item width="100%">
          <Typography variant="h3" fontSize={25}>
            {group.description}
          </Typography>
        </Grid>
      </Grid>
    </Paper>
  );
};
