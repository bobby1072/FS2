import React from "react";
import { GroupPositionModel } from "../../models/GroupPositionModel";
import { Autocomplete, Chip, Grid, TextField, Typography } from "@mui/material";

export const PositionSearchBar: React.FC<{
  positions: GroupPositionModel[];
}> = ({ positions }) => {
  return (
    <Autocomplete
      renderOption={(props, option, { selected }) => {
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
      }}
      getOptionLabel={(option) => option.name}
      options={positions}
      renderInput={(params) => {
        return <TextField {...params} label="Position" />;
      }}
    />
  );
};
