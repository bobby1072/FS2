import MUIDataTable, { MUIDataTableColumnDef, MUIDataTableOptions } from "mui-datatables";
import { RuntimePartialGroupCatchModel } from "../../models/IGroupCatchModel";
import { usePersistedState } from "../../common/hooks/PersistedState";
import { FormControl, InputLabel, MenuItem, Select, Typography } from "@mui/material";
import { prettyDateWithTime } from "../../utils/DateTime";

enum LeaderBoardType {
    Heaviest = "Heaviest",
    Longest = "Longest",
    MostRecent = "Most Recent",
}

export const PartialGroupCatchLeaderBoard: React.FC<{ partialCatches?: RuntimePartialGroupCatchModel[] }> = ({ partialCatches = [] }) => {
    const [leaderBoardType, setLeaderBoardType] = usePersistedState<LeaderBoardType>("catchLeaderBoardType", LeaderBoardType.MostRecent);
    const columns: MUIDataTableColumnDef[] = [
        {
            name: "species",
            label: "Species",
            options: {
                filter: true,
                sort: false
            }
        }, {
            name: "latitude",
            label: "Latitude",
            options: {
                filter: true,
                display: false,
                sort: false
            }
        }, {
            name: "longitude",
            label: "Longitude",
            options: {
                filter: true,
                display: false,

                sort: false
            }
        }, {
            name: "caughtAt",
            label: "Caught At",
            options: {
                filter: true,
                customBodyRender: (val: Date) =>
                    <Typography>
                        {prettyDateWithTime(val)}
                    </Typography>,
                sort: false
            }
        }, {
            name: "weight",
            label: "Weight",
            options: {
                filter: true,
                sort: false,
                customBodyRender: (val) => <Typography>{val} lbs</Typography>
            }
        }, {
            name: "length",
            label: "Length",
            options: {
                filter: true,
                sort: false,
                customBodyRender: (val) => <Typography>{val} cm</Typography>
            }
        },
        {
            name: "id",
            label: "Id",
            options: {
                display: "excluded",
                sort: false,
                filter: false
            }
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
        print: false,
        viewColumns: true,
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
        customToolbar: () =>
            <div style={{ padding: 0.1 }}>
                <FormControl >
                    <InputLabel>
                        Leader board type
                    </InputLabel>
                    <Select
                        fullWidth
                        value={leaderBoardType}
                        label="Leader board type"
                        onChange={(e) => setLeaderBoardType(e.target.value as LeaderBoardType)}
                    >
                        <MenuItem value={LeaderBoardType.MostRecent}>Most recent</MenuItem>
                        <MenuItem value={LeaderBoardType.Heaviest}>Heaviest</MenuItem>
                        <MenuItem value={LeaderBoardType.Longest}>Longest</MenuItem>
                    </Select>
                </FormControl>
            </div>
    };
    return <MUIDataTable data={partialCatches.sort((a, b) => {
        if (leaderBoardType === LeaderBoardType.MostRecent) {
            return b.caughtAt.getTime() - a.caughtAt.getTime();
        } else if (leaderBoardType === LeaderBoardType.Heaviest) {
            return b.weight - a.weight;
        }
        return b.length - a.length;
    })} columns={columns} title={"Group catch leader board"} options={options} />;
};