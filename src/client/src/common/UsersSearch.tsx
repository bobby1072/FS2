import { Autocomplete, IconButton, TextField } from "@mui/material";
import { UserModel } from "../models/UserModel";
import { useSearchUsers } from "./hooks/SearchUsers";
import { Close } from "@mui/icons-material";
import { useEffect, useState } from "react";

export const UsersSearch: React.FC<{
  filter?: (
    value: Omit<UserModel, "email">,
    index: number,
    array: Omit<UserModel, "email">[]
  ) => boolean;
  value?: Omit<UserModel, "email">;
  onChange: (value?: Omit<UserModel, "email">) => void;
}> = ({ filter = () => true, value, onChange }) => {
  const { data, isLoading, mutate, reset } = useSearchUsers();
  const [searchTerm, setSearchTerm] = useState("");
  useEffect(() => {
    reset();
    mutate({ searchTerm });
  }, [searchTerm, reset, mutate]);
  if (value) {
    return (
      <TextField
        label={"Person"}
        disabled
        value={value.username}
        variant="outlined"
        InputProps={{
          endAdornment: (
            <IconButton
              aria-label="clear"
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
      options={data?.filter(filter) || []}
      onChange={(_, option) => {
        onChange(option ?? undefined);
      }}
      getOptionLabel={(option) => option.username}
      onInputChange={(e, value, reason) => {
        if (reason === "input" && e?.type === "change") {
          setSearchTerm(value);
        } else if (reason === "clear") {
          setSearchTerm("");
        }
      }}
      noOptionsText={
        isLoading
          ? "Loading..."
          : data
          ? "No results"
          : "Start typing to search"
      }
      isOptionEqualToValue={(option, value) => option.id! === value.id!}
      renderInput={(params) => (
        <TextField
          {...params}
          variant="outlined"
          label="Person"
          InputLabelProps={{ shrink: true }}
          inputProps={{ ...params.inputProps }}
          size="medium"
        />
      )}
    />
  );
};
