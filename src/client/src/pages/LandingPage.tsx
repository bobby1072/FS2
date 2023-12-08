import { Grid, Button, Paper } from "@mui/material";
import { useNavigate } from "react-router-dom";
export const LandingPage: React.FC = () => {
  const navigate = useNavigate();
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
              onClick={() => {
                navigate("/account");
              }}
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
