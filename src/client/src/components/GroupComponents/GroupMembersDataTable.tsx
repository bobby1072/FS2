import MUIDataTable, {
  MUIDataTableColumnDef,
  MUIDataTableOptions,
} from "mui-datatables";
import { Add as AddIcon } from "@mui/icons-material";
import { IGroupMemberModel } from "../../models/IGroupMemberModel";
import { IGroupPositionModel } from "../../models/IGroupPositionModel";
import { IUserModel } from "../../models/IUserModel";
import Avatar from "react-avatar";
import { Grid, IconButton, Tooltip } from "@mui/material";
import { useEffect, useState } from "react";
import { AddMemberModal } from "./AddMemberModal";
import { useDeleteGroupMember } from "./hooks/DeleteGroupMember";
import { YesOrNoModal } from "../../common/YesOrNoModal";
import { useSnackbar } from "notistack";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import { useGetAllMembers } from "./hooks/GetFullGroup";
import { Loading } from "../../common/Loading";
import {
  PermissionActions,
  PermissionFields,
  useCurrentPermissionManager,
} from "../../common/contexts/AbilitiesContext";

interface GroupMemberRowItem {
  username: string;
  id: string;
  name?: string;
  position?: string;
  potentialAvatar: {
    initials?: string;
    email?: string;
  };
}

const mapBaseDataToRowItems = (
  members: IGroupMemberModel[],
  positions: IGroupPositionModel[],
  leader?: IUserModel
): GroupMemberRowItem[] => {
  const rowItems: GroupMemberRowItem[] = [];
  if (leader) {
    rowItems.push({
      name: leader.name ?? "",
      id: leader.id ?? "",
      username: leader.username ?? undefined,
      position: "Leader",
      potentialAvatar: {
        initials: leader.name
          ?.split(" ")
          .map((x) => x[0])
          .join(""),
        email: leader.email,
      },
    });
  }
  for (let i = 0; i < members.length; i++) {
    const localMember = members[i];
    const position = positions.find((p) => p.id === localMember.positionId);
    const initials = localMember.user?.name
      ?.split(" ")
      .map((x) => x[0])
      .join("");
    rowItems.push({
      name: localMember.user?.name ?? "",
      username: localMember.user?.username ?? "",
      position: position?.name,
      id: localMember.id?.toString()!,
      potentialAvatar: {
        initials: initials,
        email: localMember.user?.email ?? undefined,
      },
    });
  }
  return rowItems;
};

export const GroupMembersDataTable: React.FC<{
  leader: IUserModel;
  groupId: string;
  positions: IGroupPositionModel[];
}> = ({ leader, positions, groupId }) => {
  const { data: members } = useGetAllMembers(groupId);
  const rowItems = mapBaseDataToRowItems(
    members ?? [],
    positions ?? [],
    leader
  );
  const [memberToDeleteId, setMemberToDeleteId] = useState<string>();
  const {
    mutate: deleteMember,
    isLoading: deletingMember,
    reset,
    data: deletedMember,
    error: deleteMemberError,
  } = useDeleteGroupMember();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (deletedMember) {
      setMemberToDeleteId(undefined);
      enqueueSnackbar("Member deleted", { variant: "error" });
    }
  }, [deletedMember, enqueueSnackbar, setMemberToDeleteId]);
  const [addMemberModalOpen, setAddMemberModalOpen] = useState<
    boolean | IGroupMemberModel
  >(false);
  const { permissionManager } = useCurrentPermissionManager();
  if (!members) return <Loading />;
  const columns: MUIDataTableColumnDef[] = [
    {
      name: "potentialAvatar",
      label: " ",
      options: {
        sort: false,
        customBodyRender: (value: GroupMemberRowItem["potentialAvatar"]) => {
          return (
            <Grid container spacing={1} alignItems="center">
              <Grid item xs={5} md={1} sm={3}>
                <Avatar
                  email={value.email}
                  initials={value.email ? undefined : value.initials}
                  size={"30"}
                  round={"4px"}
                />
              </Grid>
            </Grid>
          );
        },
      },
    },
    {
      name: "name",
      label: "Name",
      options: {
        filter: true,
        sort: true,
      },
    },
    {
      name: "username",
      label: "Username",
      options: {
        filter: true,
        sort: false,
      },
    },
    {
      name: "position",
      label: "Position",
      options: {
        filter: true,
        sort: false,
      },
    },
    {
      name: "id",
      label: " ",
      options: {
        display: permissionManager.Can(
          PermissionActions.Manage,
          groupId,
          PermissionFields.GroupMember
        )
          ? true
          : "excluded",
        customBodyRender: (id) => {
          return id === leader.id ? null : (
            <Grid
              container
              justifyContent="center"
              alignItems={"center"}
              direction="row"
            >
              <Grid item>
                <IconButton
                  color="primary"
                  onClick={() => {
                    setAddMemberModalOpen(
                      members.find((x) => x.id === id) ?? false
                    );
                  }}
                >
                  <EditIcon />
                </IconButton>
              </Grid>
              <Grid item>
                <IconButton
                  color="primary"
                  disabled={deletingMember}
                  onClick={() => {
                    setMemberToDeleteId(id);
                  }}
                >
                  <DeleteIcon />
                </IconButton>
              </Grid>
            </Grid>
          );
        },
        viewColumns: false,
      },
    },
  ];
  const options: MUIDataTableOptions = {
    elevation: 2,
    filter: false,
    selectableRows: "none",
    tableBodyHeight: "50vh",
    responsive: "vertical",
    resizableColumns: false,
    rowsPerPage: 15,
    download: false,
    customToolbar: () =>
      permissionManager.Can(
        PermissionActions.Manage,
        groupId,
        PermissionFields.GroupMember
      ) && (
        <>
          <Tooltip title={"Add member"}>
            <IconButton
              size="large"
              onClick={() => setAddMemberModalOpen(true)}
            >
              <AddIcon />
            </IconButton>
          </Tooltip>
        </>
      ),
    print: false,
    viewColumns: false,
    searchPlaceholder: "Search",
    customSearch: (searchQuery, row) => {
      return row.some((col) => {
        if (typeof col === "string") {
          return col
            .toLocaleLowerCase()
            .includes(searchQuery.toLocaleLowerCase());
        }
        return false;
      });
    },
  };
  const memberToDeleteFull = members.find(
    (x) => x.id === memberToDeleteId
  )?.user;
  return (
    <>
      <MUIDataTable
        title="Group members"
        data={rowItems}
        columns={columns}
        options={options}
      />
      {addMemberModalOpen === true && (
        <AddMemberModal
          closeModal={() => setAddMemberModalOpen(false)}
          positions={positions}
          groupId={groupId}
          existingMemberIds={[...members!.map((x) => x.userId), leader!.id!]}
        />
      )}
      {addMemberModalOpen && addMemberModalOpen !== true && (
        <AddMemberModal
          closeModal={() => setAddMemberModalOpen(false)}
          positions={positions}
          defaultValue={addMemberModalOpen}
          groupId={groupId}
          existingMemberIds={[...members!.map((x) => x.userId), leader!.id!]}
        />
      )}
      {memberToDeleteId && (
        <YesOrNoModal
          closeModal={() => setMemberToDeleteId(undefined)}
          question={
            <>
              Are you sure you want to remove{" "}
              {memberToDeleteFull?.username ? (
                <strong>{memberToDeleteFull.username}</strong>
              ) : (
                "this person"
              )}{" "}
              from the group?
            </>
          }
          allErrors={deleteMemberError ?? undefined}
          yesAction={() => {
            reset();
            deleteMember({ groupMemberId: memberToDeleteId });
          }}
          saveDisabled={deletingMember}
        />
      )}
    </>
  );
};
