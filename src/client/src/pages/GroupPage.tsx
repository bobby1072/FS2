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
import { SnackbarProvider } from "notistack";
import { GroupModel } from "../models/GroupModel";

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
  const [groupViewChoice, setGroupViewChoice] = useState<GroupQueryChoice>(
    GroupQueryChoice.AllListed
  );
  const [groupFilterString, setGroupFilterString] = useState<string>();
  const { data: totalGroupCount, error: countError } = useGetGroupCount();
  const {
    data: listedGroups,
    refetch: listedGroupsRefetch,
    isLoading,
    error: listedGroupsError,
  } = useGetAllGroupsChoiceGroup(
    groupStartIndex === 1 ? 0 : (groupStartIndex - 1) * groupSeeCount,
    groupSeeCount,
    groupViewChoice
  );
  const queryClient = useQueryClient();
  const [createNewGroupModal, setCreateNewGroupModal] = useState<
    boolean | GroupModel
  >(false);
  useEffect(() => {
    queryClient.removeQueries(Constants.QueryKeys.GetGroupsWithChoice);
    listedGroupsRefetch();
  }, [
    groupSeeCount,
    groupStartIndex,
    queryClient,
    listedGroupsRefetch,
    groupViewChoice,
  ]);
  const isError = (listedGroupsError as any) || (countError as any);
  return (
    <PageBase>
      <SnackbarProvider>
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
            <Grid item width="100%">
              <Paper>
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
                        setGroupsIndexing({
                          groupStartIndex: 1,
                          groupSeeCount: 5,
                        });
                        setGroupFilterString(e.target.value);
                      }}
                    />
                  </Grid>
                  <Grid item width="30%" sx={{ padding: 1 }}>
                    <FormControl fullWidth>
                      <InputLabel id="demo-simple-select-disabled-label">
                        Type of group
                      </InputLabel>
                      <Select
                        fullWidth
                        labelId="demo-simple-select-disabled-label"
                        id="demo-simple-select-disabled"
                        value={groupViewChoice}
                        label="Type of group"
                        onChange={(e) => {
                          setGroupsIndexing({
                            groupStartIndex: 1,
                            groupSeeCount: 5,
                          });
                          setGroupViewChoice(e.target.value as any);
                        }}
                      >
                        <MenuItem value={GroupQueryChoice.AllListed}>
                          All listed groups
                        </MenuItem>
                        <MenuItem value={GroupQueryChoice.SelfLead}>
                          Self lead groups
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
                {listedGroups
                  ?.filter((x) =>
                    groupFilterString
                      ? x.name
                          .toLocaleLowerCase()
                          .includes(groupFilterString.toLowerCase()) ||
                        groupFilterString === x.id
                      : true
                  )
                  .map((x) => (
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
                  <Loading />
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
            group={
              createNewGroupModal === true ? undefined : createNewGroupModal
            }
          />
        )}
      </SnackbarProvider>
    </PageBase>
  );
};
