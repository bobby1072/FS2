import { Button, Grid } from "@mui/material";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import { useGetAllListedGroups } from "../components/GroupComponents/hooks/GetAllListedGroups";
import { GroupTab } from "../components/GroupComponents/GroupTab";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";

export const AllGroupDisplayPage: React.FC = () => {
  const { data, isLoading } = useGetAllListedGroups();
  if (isLoading && !data) return <Loading fullScreen />;
  return (
    <PageBase>
      <AppAndDraw>
        <Grid
          container
          direction="column"
          justifyContent="center"
          alignItems="center"
          spacing={2}
        >
          <Grid item width="100%" justifyContent="flex-end">
            <Button variant="contained" color="primary">
              Create new group
            </Button>
          </Grid>
          {data?.map((x) => (
            <Grid item width="60%" key={x.id}>
              <GroupTab group={x} />
            </Grid>
          ))}
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
