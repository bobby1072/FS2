import React from "react";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import {
  Autocomplete,
  Chip,
  Grid,
  IconButton,
  TextField,
  Typography,
} from "@mui/material";
import { Close } from "@mui/icons-material";
const RenderPosition: React.FC<{ option: GroupPositionModel }> = ({
  option,
}) => {
  return (
    <Grid container alignItems="center" spacing={1.5}>
      <Grid item width="100%">
        <Typography>{option.name}</Typography>
      </Grid>
      {Object.entries(option)
        .filter(([key, val]) => val === true)
        .map(([key, val]) => {
          return <Chip key={key} label={key} color={"primary"} />;
        })}
    </Grid>
  );
};
export const PositionSearchBar: React.FC<{
  positions: GroupPositionModel[];
  position?: GroupPositionModel;
  onChange: (position?: GroupPositionModel) => void;
}> = ({ positions, position, onChange }) => {
  if (position) {
    return (
      <TextField
        label="Position"
        variant="outlined"
        disabled
        value={<RenderPosition option={position} />}
        InputProps={{
          endAdornment: (
            <IconButton
              color="inherit"
              size="small"
              onClick={() => {
                onChange(undefined);
              }}
            >
              <Close fontSize="inherit" />
            </IconButton>
          ),
        }}
      />
    );
  }
  return (
    <Autocomplete
      renderOption={(_, option) => {
        return <RenderPosition option={option} />;
      }}
      onChange={(_, option) => {
        onChange(option ?? undefined);
      }}
      noOptionsText={
        positions.length > 0 ? "No results" : "No positions to pick from"
      }
      isOptionEqualToValue={(option, value) => option.id === value?.id}
      getOptionLabel={(option) => option.name}
      options={positions}
      renderInput={(params) => {
        return (
          <TextField
            {...params}
            label="Position"
            variant="outlined"
            InputLabelProps={{ shrink: true }}
            inputProps={{ ...params.inputProps }}
            size="medium"
          />
        );
      }}
    />
  );
};
