import { Grid } from "@mui/material";
import { PageBase } from "../common/PageBase";
import { useCurrentUser } from "../common/UserContext";
import Avatar from "react-avatar";

export const AccountPage: React.FC = () => {
  const { email } = useCurrentUser();
  return (
    <PageBase>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        direction="column"
      >
        <Grid item>
          <Avatar {...{ email }} />
        </Grid>
      </Grid>
    </PageBase>
  );
};
