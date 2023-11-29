import { Grid, Button, Paper } from "@mui/material";
import { useAuthentication } from "../common/login/Authentication";
import { useLocation } from "react-router-dom";
import { useCallback } from "react";
export const LandingPage: React.FC<{ redirectUri: string }> = ({
  redirectUri,
}) => {
  const { signIn } = useAuthentication();
  const location = useLocation();

  const doLogin = useCallback(async () => {
    // TODO: Needs to be changed redirect to a homepage when made
    await signIn({ state: { targetUrl: location.pathname } });
  }, [location.pathname, signIn]);
  return (
    <div style={{ height: "100vh", width: "100%" }}>
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
              fullWidth
              onClick={doLogin}
              sx={{ padding: 2 }}
              variant="contained"
            >
              Login
            </Button>
          </Paper>
        </Grid>
      </Grid>
    </div>
  );
};
