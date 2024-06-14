import { Grid, IconButton, Paper, Typography } from "@mui/material";
import { PageBase } from "../common/PageBase";
import { useCurrentUser } from "../common/contexts/UserContext";
import Avatar from "react-avatar";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import EditIcon from "@mui/icons-material/Edit";
import { useState } from "react";
import { EditUsernameModal } from "../components/AcountComponents/EditUsernameModal";
import { useParams } from "react-router-dom";
import { useGetUserQuery } from "../components/AcountComponents/hooks/GetUser";
import { Loading } from "../common/Loading";
import { ErrorComponent } from "../common/ErrorComponent";
import { useGetCatchesForUserQuery } from "../components/AcountComponents/hooks/GetCatchesForUser";
import { GroupCatchesMap } from "../components/CatchComponents/GroupCatchesMap";
import { PartialGroupCatchLeaderBoard } from "../components/CatchComponents/PartialGroupCatchLeaderBoard";
import { ApiException } from "../common/ApiException";

export const IndividualAccountPage: React.FC = () => {
  const { id: userId } = useParams<{ id: string }>();
  const {
    data: foundUser,
    isLoading: userLoading,
    error: userError,
  } = useGetUserQuery(userId!);
  const { id: selfId } = useCurrentUser();
  const { data: userCatches, error: userCatchesError } =
    useGetCatchesForUserQuery(userId!);
  const [editUsernameModal, setEditUsernameModal] = useState<boolean>(false);
  const isSelfPage = selfId === userId;
  if (userLoading) return <Loading fullScreen />;
  else if (userError) return <ErrorComponent error={userError} fullScreen />;
  else if (!foundUser) return <ErrorComponent fullScreen />;
  const initials = foundUser.name
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
          spacing={2}
          textAlign="center"
        >
          <Paper elevation={2} sx={{ padding: 6 }}>
            <Grid item>
              {isSelfPage ? (
                <Avatar email={foundUser.email!} />
              ) : (
                <Avatar initials={initials} />
              )}
            </Grid>
            <Grid item>
              <Typography fontSize={30}>{foundUser.name}</Typography>
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
                  <Typography fontSize={23}>
                    <strong>Username: </strong>
                    {foundUser.username}
                  </Typography>
                </Grid>
                {isSelfPage && (
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
                )}
              </Grid>
            </Grid>
            {isSelfPage && (
              <Grid item>
                <Typography fontSize={20}>
                  <strong>Email: </strong>
                  {foundUser.email}
                </Typography>
              </Grid>
            )}
          </Paper>
          {userCatchesError || (userCatches && userCatches?.length < 1) ? (
            <Grid item width={"100%"}>
              <ErrorComponent
                error={
                  !!userCatchesError
                    ? userCatchesError
                    : new ApiException("No catches to view")
                }
              />
            </Grid>
          ) : (
            userCatches && (
              <>
                <Grid item width="100%">
                  <GroupCatchesMap groupCatches={userCatches} />
                </Grid>
                <Grid item width="100%">
                  <PartialGroupCatchLeaderBoard partialCatches={userCatches} />
                </Grid>
              </>
            )
          )}
        </Grid>
      </AppAndDraw>
      {editUsernameModal && (
        <EditUsernameModal
          closeModal={() => setEditUsernameModal(false)}
          currentUsername={foundUser.username!}
        />
      )}
    </PageBase>
  );
};
