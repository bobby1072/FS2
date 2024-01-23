import { Grid, Button, Paper } from "@mui/material";
import { Navigate, useLocation } from "react-router-dom";
import { useAuthentication } from "../common/contexts/AuthenticationContext";
export const LandingPage: React.FC = () => {
  const { user, signIn } = useAuthentication();
  const location = useLocation();
  const targetUrl =
    (location.state as { targetUrl?: string } | undefined)?.targetUrl || "";

  if (user) return <Navigate to={targetUrl} />;

  return (
    <div style={{ minHeight: "101vh", width: "100%" }}>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        width="100%"
        style={{ height: "100%" }}
      >
        <Grid item width="10%">
          <Paper>
            <Button
              variant="contained"
              color="primary"
              fullWidth
              onClick={() => signIn(targetUrl)}
              sx={{ fontWeight: 700 }}
            >
              Login
            </Button>
          </Paper>
        </Grid>
      </Grid>
    </div>
  );
};
