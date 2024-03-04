import { useParams } from "react-router-dom";
import { AppAndDraw } from "../common/AppBar/AppAndDraw";
import { PageBase } from "../common/PageBase";
import { useGetFullGroup } from "../components/GroupComponents/hooks/GetFullGroup";
import { Box, Grid, Paper, Typography } from "@mui/material";
import { Loading } from "../common/Loading";

export const IndividualGroupPage: React.FC = () => {
  const { id: groupId } = useParams<{ id: string }>();
  const { data } = useGetFullGroup(groupId);
  if (!data) return <Loading fullScreen />;
  const {
    name: groupName,
    emblem: groupEmblem,
    description: groupDescription,
  } = data;
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
          <Grid item width="60%">
            <Paper elevation={2}>
              <Grid
                container
                direction={"column"}
                justifyContent="center"
                alignItems="center"
                padding={4}
                spacing={2}
              >
                <Grid item width="100%">
                  <Typography variant="h3" textAlign="center" fontSize={50}>
                    {groupName}
                  </Typography>
                </Grid>
                {groupEmblem && (
                  <Grid item>
                    <Box
                      component="img"
                      sx={{
                        border: "0.1px solid #999999",
                        maxHeight: "80vh",
                        width: "100%",
                      }}
                      src={`data:image/jpeg;base64,${groupEmblem}`}
                      alt={`emblem: ${groupId}`}
                    />
                  </Grid>
                )}
                <Grid item>
                  <Typography
                    variant="subtitle2"
                    textAlign="center"
                    fontSize={17}
                  >
                    <strong>Id: </strong>
                    {groupId}
                  </Typography>
                </Grid>
                {groupDescription && (
                  <Grid item>
                    <Typography
                      variant="subtitle2"
                      textAlign="center"
                      fontSize={20}
                    >
                      <strong>Description: </strong>
                      {groupDescription}
                    </Typography>
                  </Grid>
                )}
              </Grid>
            </Paper>
          </Grid>
        </Grid>
      </AppAndDraw>
    </PageBase>
  );
};
