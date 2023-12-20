import { Button, Grid } from "@mui/material";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import { useGetAllListedGroups } from "../components/GroupComponents/hooks/GetAllListedGroups";
import { GroupTab } from "../components/GroupComponents/GroupTab";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { useState } from "react";
import { CreateGroupModal } from "../components/GroupComponents/CreateGroupModal";

export const AllGroupDisplayPage: React.FC = () => {
  const { data, isLoading } = useGetAllListedGroups();
  const [createNewGroupModal, setCreateNewGroupModal] =
    useState<boolean>(false);
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
            <Button
              variant="contained"
              color="primary"
              onClick={() => {
                setCreateNewGroupModal(true);
              }}
            >
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
      {createNewGroupModal && (
        <CreateGroupModal closeModal={() => setCreateNewGroupModal(false)} />
      )}
    </PageBase>
  );
};
