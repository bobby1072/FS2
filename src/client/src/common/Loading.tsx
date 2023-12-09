import { PageBase } from "./PageBase";
import { Grid, Typography } from "@mui/material";
import LinearProgress from "@mui/material/LinearProgress";
export const Loading: React.FC<{ fullScreen?: boolean }> = ({
  fullScreen = false,
}) => {
  return fullScreen ? (
    <PageBase>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        direction="column"
        spacing={4}
        textAlign="center"
        sx={{ height: "100vh" }}
      >
        <Grid item width="100%">
          <LinearProgress />
        </Grid>
        <Grid item width="100%">
          <Typography variant="h1" fontSize={50}>
            Loading...
          </Typography>
        </Grid>
      </Grid>
    </PageBase>
  ) : (
    <Grid
      container
      justifyContent="center"
      alignItems="center"
      direction="column"
      width="100%"
      textAlign="center"
      spacing={2}
    >
      <Grid item width="100%">
        <LinearProgress />
      </Grid>
      <Grid item width="100%">
        <Typography variant="h1" fontSize={50}>
          Loading...
        </Typography>
      </Grid>
    </Grid>
  );
};
