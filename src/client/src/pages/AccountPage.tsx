import { Grid, Paper, Typography } from "@mui/material";
import { PageBase } from "../common/PageBase";
import { useCurrentUser } from "../common/UserContext";
import Avatar from "react-avatar";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";

export const AccountPage: React.FC = () => {
  const { email, name: givenName } = useCurrentUser();
  const initials = givenName
    ?.split(" ")
    .map((x) => x[0])
    .join("");
  return (
    <PageBase>
      <AppAndDraw>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          direction="column"
          textAlign="center"
        >
          <Grid item sx={{ mb: 3 }}>
            <Typography variant="h3" fontSize={50}>
              Account
            </Typography>
          </Grid>
          <Paper elevation={2} sx={{ padding: 6 }}>
            <Grid item>
              <Avatar {...{ email }} initials={initials} />
            </Grid>
            <Grid item>
              <Typography fontSize={25}>{givenName}</Typography>
            </Grid>
            <Grid item>
              <Typography fontSize={18}>Email: {email}</Typography>
            </Grid>
          </Paper>
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
