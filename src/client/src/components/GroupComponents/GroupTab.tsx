import {
  Box,
  Button,
  Grid,
  IconButton,
  Paper,
  Tooltip,
  Typography,
} from "@mui/material";
import LockIcon from "@mui/icons-material/Lock";
import LockOpenIcon from "@mui/icons-material/LockOpen";
import { IGroupModel } from "../../models/IGroupModel";
import VisibilityIcon from "@mui/icons-material/Visibility";
import VisibilityOffIcon from "@mui/icons-material/VisibilityOff";
import { useState } from "react";
import EditIcon from "@mui/icons-material/Edit";
import { prettyDateWithYear } from "../../utils/DateTime";
import { useCurrentUser } from "../../common/contexts/UserContext";
export const GroupTab: React.FC<{
  group: IGroupModel;
  openModal: () => void;
  linkToMainGroupPage?: boolean;
}> = ({ group, openModal, linkToMainGroupPage = true }) => {
  const [viewId, setViewId] = useState<boolean>(false);
  const { id: selfId } = useCurrentUser();
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
        <Grid item width={"100%"} justifyContent="flex-end" display="flex">
          {group.public ? (
            <Tooltip title={"public"}>
              <LockOpenIcon />
            </Tooltip>
          ) : (
            <Tooltip title="private">
              <LockIcon />
            </Tooltip>
          )}
        </Grid>
        {group.emblem && (
          <Grid item width="100%" minHeight={"10vh"}>
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
          </Grid>
        )}
        <Grid item width="100%">
          {!viewId && (
            <Tooltip title="Show group id">
              <IconButton onClick={(_) => setViewId((viewOn) => !viewOn)}>
                <VisibilityOffIcon />
              </IconButton>
            </Tooltip>
          )}
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
        {selfId === group.leaderId && (
          <Grid item>
            <IconButton onClick={openModal} color="primary">
              <EditIcon />
            </IconButton>
          </Grid>
        )}
        {linkToMainGroupPage && (
          <Grid item>
            <Button href={`/Group/${group.id}`} variant="contained">
              See more
            </Button>
          </Grid>
        )}
      </Grid>
    </Paper>
  );
};
