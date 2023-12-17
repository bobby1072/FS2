import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Grid, Typography } from "@mui/material";
import { useCurrentUser } from "../common/UserContext";

export function Landing() {
  const navigate = useNavigate();
  const user = useCurrentUser();
  const [userNotFound, setUserNotFound] = useState(false);
  useEffect(() => {
    if (user) {
      navigate("/Home");
    } else {
      setUserNotFound(true);
    }
  }, [navigate, setUserNotFound, user]);

  if (userNotFound) {
    return (
      <Grid item lg={6} md={6} sm={6} xs={12}>
        <Typography variant="body1" data-testid="redirect-message">
          Failed to load details
        </Typography>
      </Grid>
    );
  }

  return <Typography> You should be redirected shortly...</Typography>;
}

export default Landing;
