import MUIDataTable, {
  MUIDataTableColumnDef,
  MUIDataTableOptions,
} from "mui-datatables";
import { Add as AddIcon } from "@mui/icons-material";
import { GroupMemberModel } from "../../models/GroupMemberModel";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { UserModel } from "../../models/UserModel";
import Avatar from "react-avatar";
import { Button, Grid, IconButton, Tooltip } from "@mui/material";
import { useEffect, useState } from "react";
import { AddMemberModal } from "./AddMemberModal";
import { useDeleteGroupMember } from "./hooks/DeleteGroupMember";
import { YesOrNoModal } from "../../common/YesOrNoModal";
import { useSnackbar } from "notistack";

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
  members: GroupMemberModel[],
  positions: GroupPositionModel[],
  leader?: UserModel
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
      id: localMember.id ?? "",
      potentialAvatar: {
        initials: initials,
        email: localMember.user?.email ?? undefined,
      },
    });
  }
  return rowItems;
};

export const GroupMembersDataTable: React.FC<{
  members: GroupMemberModel[];
  leader: UserModel;
  groupId: string;
  positions: GroupPositionModel[];
}> = ({ leader, members, positions, groupId }) => {
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
  } = useDeleteGroupMember();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (deletedMember) {
      setMemberToDeleteId(undefined);
      enqueueSnackbar("Member deleted", { variant: "error" });
    }
  }, [deletedMember, enqueueSnackbar, setMemberToDeleteId]);
  const [addMemberModalOpen, setAddMemberModalOpen] = useState<boolean>(false);
  const columns: MUIDataTableColumnDef[] = [
    {
      name: "potentialAvatar",
      label: "name",
      options: {
        sort: false,
        customBodyRender: (value: GroupMemberRowItem["potentialAvatar"]) => {
          return (
            <Grid container spacing={1} alignItems="center">
              <Grid item xs={5} md={1} sm={3}>
                <Avatar
                  size={"30"}
                  email={value.email}
                  initials={value.email ? undefined : value.initials}
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
        sort: false,
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
        customBodyRender: (id) => {
          return id === leader.id ? null : (
            <Button
              variant="contained"
              onClick={() => {
                setMemberToDeleteId(id);
              }}
            >
              Delete
            </Button>
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
    customToolbar: () => (
      <>
        <Tooltip title={"Add member"}>
          <IconButton size="large" onClick={() => setAddMemberModalOpen(true)}>
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
          return col.toLowerCase().includes(searchQuery.toLowerCase());
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
      {addMemberModalOpen && (
        <AddMemberModal
          closeModal={() => setAddMemberModalOpen(false)}
          positions={positions}
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
