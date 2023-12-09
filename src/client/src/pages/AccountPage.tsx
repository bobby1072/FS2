import { Grid } from "@mui/material";
import { PageBase } from "../common/PageBase";
import { useCurrentUser } from "../common/UserContext";
import Avatar from "react-avatar";

export const AccountPage: React.FC = () => {
  const { email, name: givenName } = useCurrentUser();
  const initials = givenName
    ?.split(" ")
    .map((x) => x[0])
    .join("");
  return (
    <PageBase>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        direction="column"
      >
        <Grid item>
          <Avatar {...{ email }} initials={initials} />
        </Grid>
      </Grid>
    </PageBase>
  );
};
