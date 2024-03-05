import MUIDataTable, {
  MUIDataTableColumnDef,
  MUIDataTableOptions,
} from "mui-datatables";
import { GroupMemberModel } from "../../models/GroupMemberModel";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { UserModel } from "../../models/UserModel";
import Avatar from "react-avatar";
import { Grid } from "@mui/material";

interface GroupMemberRowItem {
  username: string;
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
  for (let i = 0; i > members.length; i++) {
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
      potentialAvatar: {
        initials: initials,
        email: localMember.user?.email ?? undefined,
      },
    });
  }
  return rowItems;
};

export const GroupMembersDataTable: React.FC<{
  members?: GroupMemberModel[];
  leader?: UserModel;
  positions?: GroupPositionModel[];
}> = ({ leader, members, positions }) => {
  const rowItems = mapBaseDataToRowItems(
    members ?? [],
    positions ?? [],
    leader
  );
  const columns: MUIDataTableColumnDef[] = [
    {
      name: "potentialAvatar",
      label: "name",
      options: {
        sort: false,
        sortDirection: "asc",
        sortDescFirst: true,
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
  ];
  const options: MUIDataTableOptions = {
    elevation: 2,
    filter: false,
    selectableRows: "none",
    responsive: "vertical",
    resizableColumns: false,
    rowsPerPage: 15,
    download: false,
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
  return (
    <MUIDataTable
      title=""
      data={rowItems}
      columns={columns}
      options={options}
    />
  );
};
