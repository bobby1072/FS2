import { Chip, Grid, Typography } from "@mui/material";
import { IGroupPositionModel } from "../../models/GroupPositionModel";

export const RenderPosition: React.FC<{ option: IGroupPositionModel }> = ({
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
          return (
            <Grid item key={`${option.id} - ${key}`}>
              <Chip key={key} label={key} color={"primary"} />
            </Grid>
          );
        })}
    </Grid>
  );
};
