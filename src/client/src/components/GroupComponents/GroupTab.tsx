import { Box, Grid, Paper, Typography } from "@mui/material";
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
        textAlign={"center"}
        spacing={2}
        padding={2}
      >
        <Grid item width="100%" minHeight={"10vh"}>
          {group.emblem && (
            <>
              <Box
                component="img"
                sx={{
                  maxHeight: "50vh",
                  width: "80%",
                }}
                src={`data:image/jpeg;base64,${group.emblem}`}
                alt={`emblem: ${group.id}`}
              />
            </>
          )}
        </Grid>
        <Grid item width="100%">
          <Typography variant="h4" fontSize={16}>
            <strong>Id: </strong>
            {group.id}
          </Typography>
        </Grid>
        <Grid item width="100%">
          <Typography variant="h2" fontSize={25}>
            <strong>Name: </strong>
            {group.name}
          </Typography>
        </Grid>
        {group.description && (
          <Grid item width="100%">
            <Typography variant="h3" fontSize={20}>
              <strong>Description: </strong>
              {group.description}
            </Typography>
          </Grid>
        )}
      </Grid>
    </Paper>
  );
};
