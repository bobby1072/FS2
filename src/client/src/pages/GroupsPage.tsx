import {
  Alert,
  Button,
  Divider,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  TextField,
  Typography,
} from "@mui/material";
import { Loading } from "../common/Loading";
import { PageBase } from "../common/PageBase";
import {
  GroupQueryChoice,
  useGetAllGroupsChoiceGroup,
  useSearchAllListedGroupsMutation,
} from "../components/GroupComponents/hooks/GetAllListedGroups";
import { GroupTab } from "../components/GroupComponents/GroupTab";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { useEffect, useState } from "react";
import { CreateGroupModal } from "../components/GroupComponents/CreateGroupModal";
import { useGetGroupCount } from "../components/GroupComponents/hooks/GetGroupCount";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import { useQueryClient } from "react-query";
import Constants from "../common/Constants";
import { IGroupModel } from "../models/IGroupModel";
import { ApiException } from "../common/ApiException";

interface IMatchRange {
  groupStartIndex: number;
  groupSeeCount: number;
}

const calcMaxPages = (len: number, matchRange: IMatchRange) =>
  Math.ceil(len / matchRange.groupSeeCount);
const retryFunc = (index: number, err: ApiException) => {
  if (err.status === 404) return false;
  return index < 3;
};
export const AllGroupDisplayPage: React.FC = () => {
  const [{ groupSeeCount, groupStartIndex }, setGroupsIndexing] =
    useState<IMatchRange>({ groupStartIndex: 1, groupSeeCount: 5 });
  const [groupViewChoice, setGroupViewChoice] = useState<GroupQueryChoice>(
    GroupQueryChoice.AllListed
  );
  const { data: totalGroupCount, error: countError } = useGetGroupCount();
  const {
    data: searchedForListedGroups,
    mutate: searchGroup,
    isLoading: searchedGroupLoading,
    error: searchedGroupError,
  } = useSearchAllListedGroupsMutation({ retry: retryFunc });
  const {
    data: listedGroups,
    refetch: listedGroupsRefetch,
    isLoading: isListedGroupsLoading,
    error: listedGroupsError,
  } = useGetAllGroupsChoiceGroup(
    groupStartIndex === 1 ? 0 : (groupStartIndex - 1) * groupSeeCount,
    groupSeeCount,
    groupViewChoice,
    {
      retry: retryFunc,
    }
  );
  const [currentGroupsToSee, setCurrentGroupsToSee] = useState<IGroupModel[]>();
  useEffect(() => {
    if (listedGroups) {
      setCurrentGroupsToSee(listedGroups);
    }
  }, [listedGroups]);
  useEffect(() => {
    if (searchedForListedGroups) {
      setCurrentGroupsToSee(searchedForListedGroups);
    }
  }, [searchedForListedGroups]);
  const queryClient = useQueryClient();
  const [createNewGroupModal, setCreateNewGroupModal] = useState<
    boolean | IGroupModel
  >(false);
  useEffect(() => {
    queryClient.removeQueries(Constants.QueryKeys.GetGroupsWithChoice);
    listedGroupsRefetch();
  }, [groupStartIndex, queryClient, listedGroupsRefetch, groupViewChoice]);
  const isError = listedGroupsError || countError || searchedGroupError;
  const isLoading = searchedGroupLoading || isListedGroupsLoading;
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
              All groups
            </Typography>
          </Grid>
          <Grid item width="100%">
            <Paper elevation={2}>
              <Grid
                container
                direction="row"
                padding={3}
                width="100%"
                alignItems="center"
              >
                <Grid item width="20%" sx={{ padding: 1 }}>
                  <TextField
                    label="Search"
                    fullWidth
                    onChange={(e) => {
                      if (!e.target.value) {
                        if (groupStartIndex !== 1) {
                          setGroupsIndexing({
                            groupStartIndex: 1,
                            groupSeeCount: 5,
                          });
                        } else {
                          queryClient.removeQueries(
                            Constants.QueryKeys.GetGroupsWithChoice
                          );
                          listedGroupsRefetch();
                        }
                      } else {
                        searchGroup({ groupName: e.target.value });
                      }
                    }}
                  />
                </Grid>
                <Grid item width="30%" sx={{ padding: 1 }}>
                  <FormControl fullWidth>
                    <InputLabel>Type of group</InputLabel>
                    <Select
                      fullWidth
                      value={groupViewChoice}
                      label="Type of group"
                      onChange={(e) => {
                        setGroupsIndexing({
                          groupStartIndex: 1,
                          groupSeeCount: 5,
                        });
                        setGroupViewChoice(e.target.value as GroupQueryChoice);
                      }}
                    >
                      <MenuItem value={GroupQueryChoice.AllListed}>
                        All listed groups
                      </MenuItem>
                      <MenuItem value={GroupQueryChoice.SelfLead}>
                        My groups
                      </MenuItem>
                    </Select>
                  </FormControl>
                </Grid>
                <Grid
                  item
                  width="35%"
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
                <Grid item width="15%">
                  <Grid
                    container
                    direction="row"
                    alignItems="center"
                    width="100%"
                    sx={{ justifyContent: "flex-end", display: "flex" }}
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
                          if (totalGroupCount && currentGroupsToSee)
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
                          if (totalGroupCount && currentGroupsToSee)
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
                </Grid>
              </Grid>
            </Paper>
          </Grid>
          <Grid item width="100%">
            <Divider />
          </Grid>
          <Grid item sx={{ mb: 1 }}></Grid>
          {currentGroupsToSee && !isLoading && !isError ? (
            <>
              {currentGroupsToSee.map((x) => (
                <Grid item width="60%" key={x.id}>
                  <GroupTab
                    group={x}
                    openModal={() => {
                      setCreateNewGroupModal(x);
                    }}
                  />
                </Grid>
              ))}
            </>
          ) : (
            <Grid item width="100%">
              {isError ? (
                <Alert severity="error" sx={{ fontSize: 20 }}>
                  {isError.message}
                </Alert>
              ) : (
                <Grid
                  container
                  width="100%"
                  minHeight="40vh"
                  justifyContent="center"
                  alignItems="center"
                >
                  <Grid item width="100%">
                    <Loading />
                  </Grid>
                </Grid>
              )}
            </Grid>
          )}
        </Grid>
      </AppAndDraw>
      {createNewGroupModal && (
        <CreateGroupModal
          closeModal={() => {
            setCreateNewGroupModal(false);
          }}
          group={createNewGroupModal === true ? undefined : createNewGroupModal}
        />
      )}
    </PageBase>
  );
};
