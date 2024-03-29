import React from "react";
import { IGroupPositionModel } from "../../models/GroupPositionModel";
import { Autocomplete, IconButton, TextField } from "@mui/material";
import { Close } from "@mui/icons-material";

export const PositionSearchBar: React.FC<{
  positions: IGroupPositionModel[];
  position?: IGroupPositionModel;
  onChange: (position?: IGroupPositionModel) => void;
}> = ({ positions, position, onChange }) => {
  if (position) {
    return (
      <TextField
        label="Position"
        variant="outlined"
        disabled
        value={position.name}
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
