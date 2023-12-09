import { Grid } from "@mui/material";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import { useGetAllListedGroups } from "../components/GroupComponents/hooks/GetAllListedGroups";
import { GroupTab } from "../components/GroupComponents/GroupTab";

export const AllGroupDisplayPage: React.FC = () => {
  const { data, isLoading } = useGetAllListedGroups();
  if (isLoading && !data) return <Loading fullScreen />;
  return (
    <PageBase>
      <Grid
        container
        direction="column"
        justifyContent="center"
        alignItems="center"
        spacing={2}
      >
        {data?.map((x) => (
          <Grid item width="60%" key={x.id}>
            <GroupTab group={x} />
          </Grid>
        ))}
      </Grid>
    </PageBase>
  );
};
