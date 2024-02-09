import { Grid, IconButton, Paper, Typography } from "@mui/material";
import { PageBase } from "../common/PageBase";
import { useCurrentUser } from "../common/UserContext";
import Avatar from "react-avatar";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import EditIcon from "@mui/icons-material/Edit";
import { useState } from "react";
import { EditUsernameModal } from "../components/AcountComponents/EditUsernameModal";

export const AccountPage: React.FC = () => {
  const { email, name: givenName, username } = useCurrentUser();
  const initials = givenName
    ?.split(" ")
    .map((x) => x[0])
    .join("");
  const [editUsernameModal, setEditUsernameModal] = useState<boolean>(false);
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
              <Avatar email={email} initials={initials} />
            </Grid>
            <Grid item>
              <Typography fontSize={30}>{givenName}</Typography>
            </Grid>
            <Grid item width="100%">
              <Grid
                container
                justifyContent="center"
                alignItems="center"
                direction="row"
                spacing={0.1}
              >
                <Grid item>
                  <Typography fontSize={23}>Username: {username}</Typography>
                </Grid>
                <Grid item>
                  <IconButton
                    onClick={() => {
                      setEditUsernameModal(true);
                    }}
                    color="primary"
                  >
                    <EditIcon />
                  </IconButton>
                </Grid>
              </Grid>
            </Grid>
            <Grid item>
              <Typography fontSize={20}>Email: {email}</Typography>
            </Grid>
          </Paper>
        </Grid>
      </AppAndDraw>
      {editUsernameModal && (
        <EditUsernameModal closeModal={() => setEditUsernameModal(false)} />
      )}
    </PageBase>
  );
};
