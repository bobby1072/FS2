import { Grid, Paper, Typography } from "@mui/material";
import { GroupModel } from "../../models/GroupModel";

export const GroupTab: React.FC<{ group: GroupModel }> = ({ group }) => {
  return (
    <Paper elevation={2}>
      <Grid
        container
        width="100%"
        direction="column"
        justifyContent="center"
        alignItems="center"
        spacing={2}
        padding={2}
      >
        <Grid item width="100%">
          <Typography variant="h4" fontSize={16}>
            {group.id}
          </Typography>
        </Grid>
        <Grid item width="100%">
          <Typography variant="h2" fontSize={30}>
            {group.name}
          </Typography>
        </Grid>
        <Grid item width="100%">
          <Typography variant="h3" fontSize={20}>
            {group.description}
          </Typography>
        </Grid>
      </Grid>
    </Paper>
  );
};
