import MUIDataTable, {
  MUIDataTableColumnDef,
  MUIDataTableOptions,
} from "mui-datatables";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { Grid, IconButton, Tooltip, Typography } from "@mui/material";
import { Add as AddIcon } from "@mui/icons-material";
import { useEffect, useState } from "react";
import { GroupPositionModal } from "./GroupPositionModal";
import EditIcon from "@mui/icons-material/Edit";
import { YesOrNoModal } from "../../common/YesOrNoModal";
import { useDeletePositionMutation } from "./hooks/DeletePosition";
import { useSnackbar } from "notistack";
import DeleteIcon from "@mui/icons-material/Delete";
import { usePersistedState } from "../../common/hooks/PersistedState";

interface GroupPositionRowItem {
  name: string;
  id: string;
  canManageGroup: boolean;
  canManageCatches: boolean;
  canManageMembers: boolean;
  canReadMembers: boolean;
  canReadCatches: boolean;
}
const mapBaseDataToRowItems = (rawData: GroupPositionModel[]) => {
  const rowItems: GroupPositionRowItem[] = [];
  for (let i = 0; i < rawData.length; i++) {
    const localItem = rawData[i];
    rowItems.push({
      id: localItem.id!,
      name: localItem.name,
      canManageGroup: localItem.canManageGroup,
      canReadCatches: localItem.canReadCatches,
      canReadMembers: localItem.canReadMembers,
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
  const [selectedRows, setSelectedRows] = usePersistedState(
    "positionTableSelectedRows",
    ["name", "canManageGroup", "canManageCatches", "canManageMembers"]
  );
  const rowItems = mapBaseDataToRowItems(positions);
  const [positionToDeleteId, setPositionToDeleteId] = useState<string>();
  const [addPositionModalOpen, setAddPositionModalOpen] = useState<
    boolean | GroupPositionModel
  >(false);
  const { enqueueSnackbar } = useSnackbar();
  const {
    data: deletedPosition,
    isLoading: deletingPosition,
    error: deletePositionError,
    reset,
    mutate: deletePositionFunc,
  } = useDeletePositionMutation();
  useEffect(() => {
    if (deletedPosition) {
      setPositionToDeleteId(undefined);
      enqueueSnackbar("Position deleted", { variant: "error" });
    }
  }, [deletedPosition, enqueueSnackbar, setPositionToDeleteId]);
  const columns: MUIDataTableColumnDef[] = [
    {
      name: "name",
      label: "Name",
      options: {
        filter: true,
        sort: true,
        display: selectedRows.includes("name"),
      },
    },
    {
      name: "canManageGroup",
      label: "Can manage the group",
      options: {
        filter: true,
        sort: true,
        display: selectedRows.includes("canManageGroup"),
        customBodyRender: (value) => {
          return value ? (
            <Typography textAlign="center">Yes</Typography>
          ) : (
            <Typography textAlign="center">No</Typography>
          );
        },
      },
    },
    {
      name: "canManageCatches",
      label: "Can manage catches",
      options: {
        filter: true,
        sort: true,
        display: selectedRows.includes("canManageCatches"),
        customBodyRender: (value) => {
          return value ? (
            <Typography textAlign="center">Yes</Typography>
          ) : (
            <Typography textAlign="center">No</Typography>
          );
        },
      },
    },
    {
      name: "canManageMembers",
      label: "Can manage members",
      options: {
        filter: true,
        sort: true,
        display: selectedRows.includes("canManageMembers"),
        customBodyRender: (value) => {
          return value ? (
            <Typography textAlign="center">Yes</Typography>
          ) : (
            <Typography textAlign="center">No</Typography>
          );
        },
      },
    },

    {
      name: "canReadMembers",
      label: "Can read members",
      options: {
        filter: true,
        sort: true,
        display: selectedRows.includes("canReadMembers"),
        customBodyRender: (value) => {
          return value ? (
            <Typography textAlign="center">Yes</Typography>
          ) : (
            <Typography textAlign="center">No</Typography>
          );
        },
      },
    },
    {
      name: "canReadCatches",
      label: "Can read catches",
      options: {
        filter: true,
        sort: true,
        display: selectedRows.includes("canReadCatches"),
        customBodyRender: (value) => {
          return value ? (
            <Typography textAlign="center">Yes</Typography>
          ) : (
            <Typography textAlign="center">No</Typography>
          );
        },
      },
    },
    {
      name: "id",
      label: " ",
      options: {
        viewColumns: false,
        customBodyRender: (id) => {
          return (
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
                    setAddPositionModalOpen(
                      positions.find((x) => x.id === id) ?? false
                    );
                  }}
                >
                  <EditIcon />
                </IconButton>
              </Grid>
              <Grid item>
                <IconButton
                  color="primary"
                  onClick={() => {
                    setPositionToDeleteId(id);
                  }}
                >
                  <DeleteIcon />
                </IconButton>
              </Grid>
            </Grid>
          );
        },
      },
    },
  ];
  const options: MUIDataTableOptions = {
    elevation: 2,
    filter: false,
    selectableRows: "none",
    responsive: "vertical",
    tableBodyHeight: "50vh",
    resizableColumns: false,
    rowsPerPage: 15,
    download: false,
    onViewColumnsChange: (changedColumn: string, action: string) => {
      if (action === "remove") {
        setSelectedRows(
          [...selectedRows].filter((column) => column !== changedColumn)
        );
      } else if (action === "add") {
        setSelectedRows([...selectedRows, changedColumn]);
      }
    },
    customToolbar: () => (
      <>
        <Tooltip title={"Add position"}>
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
    viewColumns: true,
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
      {addPositionModalOpen === true && (
        <GroupPositionModal
          closeModal={() => setAddPositionModalOpen(false)}
          groupId={groupId}
        />
      )}
      {addPositionModalOpen && addPositionModalOpen !== true && (
        <GroupPositionModal
          closeModal={() => setAddPositionModalOpen(false)}
          groupId={groupId}
          defaultValue={addPositionModalOpen}
        />
      )}
      {positionToDeleteId && (
        <YesOrNoModal
          closeModal={() => setPositionToDeleteId(undefined)}
          question={`Are you sure you want to delete this position from the group? Anyone who is assigned to this position will also be deleted.`}
          yesAction={() => {
            reset();
            deletePositionFunc({
              positionId: positionToDeleteId,
            });
          }}
          saveDisabled={deletingPosition}
          allErrors={deletePositionError ?? undefined}
        />
      )}
    </>
  );
};
