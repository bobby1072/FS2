import { Alert, Button, Divider, Grid, Paper, Typography } from "@mui/material";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import { useGetAllListedGroups } from "../components/GroupComponents/hooks/GetAllListedGroups";
import { GroupTab } from "../components/GroupComponents/GroupTab";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { useEffect, useState } from "react";
import { CreateGroupModal } from "../components/GroupComponents/CreateGroupModal";
import { useGetGroupCount } from "../components/GroupComponents/hooks/GetGroupCount";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import { useQueryClient } from "react-query";
import Constants from "../common/Constants";

interface IMatchRange {
  groupStartIndex: number;
  groupSeeCount: number;
}

const calcMaxPages = (len: number, matchRange: IMatchRange) => {
  const remainder = len % matchRange.groupSeeCount;
  return (len - remainder) / matchRange.groupSeeCount + 1;
};

export const AllGroupDisplayPage: React.FC = () => {
  const [{ groupSeeCount, groupStartIndex }, setGroupsIndexing] =
    useState<IMatchRange>({ groupStartIndex: 1, groupSeeCount: 5 });
  const {
    data: totalGroupCount,
    isLoading: groupCountLoading,
    error: countError,
  } = useGetGroupCount();
  const {
    data: listedGroups,
    refetch: listedGroupsRefetch,
    isLoading: listedGroupsLoading,
    error: listedGroupsError,
  } = useGetAllListedGroups(
    groupStartIndex === 1 ? 0 : (groupStartIndex - 1) * groupSeeCount,
    groupSeeCount
  );
  const queryClient = useQueryClient();
  const [createNewGroupModal, setCreateNewGroupModal] =
    useState<boolean>(false);
  useEffect(() => {
    queryClient.removeQueries(Constants.QueryKeys.GetAllListedGroups);
    listedGroupsRefetch();
  }, [groupSeeCount, groupStartIndex, queryClient, listedGroupsRefetch]);
  const isLoading = groupCountLoading || listedGroupsLoading;
  const isError = countError || (listedGroupsError as any);
  return (
    <PageBase>
      <AppAndDraw>
        <Grid
          container
          direction="column"
          justifyContent="center"
          alignItems="center"
          spacing={3}
        >
          <Grid item width="100%">
            <Typography variant="h3" fontSize={50}>
              All listed groups
            </Typography>
          </Grid>
          <Grid
            item
            width="100%"
            sx={{ display: "flex", justifyContent: "flex-end" }}
          >
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
          <Grid item width="100%">
            <Paper>
              <Grid
                container
                direction="row"
                padding={3}
                width="100%"
                alignItems="center"
              >
                <Grid item sx={{ marginRight: 1 }}>
                  <Typography variant="subtitle2" fontSize={18}>
                    {`groups ${
                      groupStartIndex === 1
                        ? groupStartIndex
                        : (groupStartIndex - 1) * groupSeeCount
                    }-${groupStartIndex * groupSeeCount}`}
                  </Typography>
                </Grid>
                <Grid item>
                  <div
                    style={{ cursor: "pointer" }}
                    onClick={() => {
                      if (totalGroupCount && listedGroups)
                        setGroupsIndexing((_) =>
                          groupStartIndex !== 1
                            ? {
                                groupStartIndex: _.groupStartIndex - 1,
                                groupSeeCount: _.groupSeeCount,
                              }
                            : _
                        );
                    }}
                  >
                    <ArrowBackIcon fontSize="medium" />
                  </div>
                </Grid>
                <Grid item>
                  <div
                    style={{ cursor: "pointer" }}
                    aria-label="next-page"
                    onClick={() => {
                      if (totalGroupCount && listedGroups)
                        setGroupsIndexing((_) =>
                          calcMaxPages(totalGroupCount, {
                            groupSeeCount,
                            groupStartIndex,
                          }) !== groupStartIndex
                            ? {
                                groupStartIndex: _.groupStartIndex + 1,
                                groupSeeCount: _.groupSeeCount,
                              }
                            : _
                        );
                    }}
                  >
                    <ArrowForwardIcon fontSize="medium" />
                  </div>
                </Grid>
              </Grid>
            </Paper>
          </Grid>
          <Grid item width="100%">
            <Divider />
          </Grid>
          <Grid item sx={{ mb: 1 }}></Grid>
          {listedGroups && !isLoading && !isError ? (
            <>
              {listedGroups?.map((x) => (
                <Grid item width="60%" key={x.id}>
                  <GroupTab group={x} />
                </Grid>
              ))}
            </>
          ) : (
            <Grid item width="100%">
              {isError ? (
                <Alert severity="error" sx={{ fontSize: 20 }}>
                  {"response" in isError &&
                  "status" in isError.response &&
                  isError.response.status === 404
                    ? "No groups found"
                    : isError.message}
                </Alert>
              ) : (
                <Loading />
              )}
            </Grid>
          )}
        </Grid>
      </AppAndDraw>
      {createNewGroupModal && (
        <CreateGroupModal closeModal={() => setCreateNewGroupModal(false)} />
      )}
    </PageBase>
  );
};
