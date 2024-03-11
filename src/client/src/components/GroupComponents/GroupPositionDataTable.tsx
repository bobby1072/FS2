import MUIDataTable, {
  MUIDataTableColumnDef,
  MUIDataTableOptions,
} from "mui-datatables";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { IconButton, Tooltip } from "@mui/material";
import { Add as AddIcon } from "@mui/icons-material";
import { useState } from "react";
import { GroupPositionModal } from "./GroupPositionModal";

interface GroupPositionRowItem {
  name: string;
  canManageGroup: boolean;
  canManageCatches: boolean;
  canManageMembers: boolean;
}
const mapBaseDataToRowItems = (rawData: GroupPositionModel[]) => {
  const rowItems: GroupPositionRowItem[] = [];
  for (let i = 0; i < rawData.length; i++) {
    const localItem = rawData[i];
    rowItems.push({
      name: localItem.name,
      canManageGroup: localItem.canManageGroup,
      canManageCatches: localItem.canManageCatches,
      canManageMembers: localItem.canManageMembers,
    });
  }
  return rowItems;
};
export const GroupPositionDataTable: React.FC<{
  positions: GroupPositionModel[];
  groupId: string;
}> = ({ positions, groupId }) => {
  const rowItems = mapBaseDataToRowItems(positions);
  const [addPositionModalOpen, setAddPositionModalOpen] =
    useState<boolean>(false);
  const columns: MUIDataTableColumnDef[] = [
    {
      name: "name",
      label: "Name",
      options: {
        filter: true,
        sort: true,
      },
    },
    {
      name: "canManageGroup",
      label: "Can manage the group",
      options: {
        filter: true,
        sort: true,
      },
    },
    {
      name: "canManageCatches",
      label: "Can manage catches",
      options: {
        filter: true,
        sort: true,
      },
    },
    {
      name: "canManageMembers",
      label: "Can manage members",
      options: {
        filter: true,
        sort: true,
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
    customToolbar: () => (
      <>
        <Tooltip title={"Add member"}>
          <IconButton
            size="large"
            onClick={() => setAddPositionModalOpen(true)}
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
          return col.toLowerCase().includes(searchQuery.toLowerCase());
        }
        return false;
      });
    },
  };
  return (
    <>
      <MUIDataTable
        title={"Group positions"}
        data={rowItems}
        columns={columns}
        options={options}
      />
      {addPositionModalOpen && (
        <GroupPositionModal
          closeModal={() => setAddPositionModalOpen(false)}
          groupId={groupId}
        />
      )}
    </>
  );
};
