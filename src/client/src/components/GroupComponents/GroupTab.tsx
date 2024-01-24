import {
  Box,
  Grid,
  IconButton,
  Paper,
  Tooltip,
  Typography,
} from "@mui/material";
import { GroupModel } from "../../models/GroupModel";
import VisibilityIcon from "@mui/icons-material/Visibility";
import VisibilityOffIcon from "@mui/icons-material/VisibilityOff";
import { useState } from "react";
import { prettyDateWithYear } from "../../utils/DateTime";
export const GroupTab: React.FC<{ group: GroupModel }> = ({ group }) => {
  const [viewId, setViewId] = useState<boolean>(false);
  return (
    <Paper elevation={2}>
      <Grid
        container
        width="100%"
        direction="column"
        justifyContent="center"
        alignItems="center"
        textAlign={"center"}
        spacing={2}
        padding={2}
      >
        <Grid item width="100%" minHeight={"10vh"}>
          {group.emblem && (
            <>
              <Box
                component="img"
                sx={{
                  border: "0.1px solid #999999",
                  maxHeight: "50vh",
                  width: "80%",
                }}
                src={`data:image/jpeg;base64,${group.emblem}`}
                alt={`emblem: ${group.id}`}
              />
            </>
          )}
        </Grid>
        <Grid item width="100%">
          {
            <Tooltip title="Show group id">
              <IconButton onClick={(_) => setViewId((viewOn) => !viewOn)}>
                {!viewId && <VisibilityOffIcon />}
              </IconButton>
            </Tooltip>
          }
          {viewId && (
            <Typography variant="h4" fontSize={15}>
              <Tooltip title="Hide group id">
                <IconButton onClick={(_) => setViewId((viewOn) => !viewOn)}>
                  <VisibilityIcon />
                </IconButton>
              </Tooltip>
              <strong>Id: </strong>
              {group.id}
            </Typography>
          )}
        </Grid>
        <Grid item width="100%">
          <Typography variant="h2" fontSize={25}>
            <strong>Name: </strong>
            {group.name}
          </Typography>
        </Grid>
        {group.description && (
          <Grid item width="100%">
            <Typography variant="h3" fontSize={20}>
              <strong>Description: </strong>
              {group.description}
            </Typography>
          </Grid>
        )}
        <Grid item width="100%">
          <Typography variant="h3" fontSize={18}>
            <strong>Created at: </strong>
            {prettyDateWithYear(new Date(Date.parse(group.createdAt)))}
          </Typography>
        </Grid>
      </Grid>
    </Paper>
  );
};
