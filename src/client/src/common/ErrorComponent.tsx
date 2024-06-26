import { Alert, Grid, Typography } from "@mui/material";
import React from "react";
import { FieldErrors } from "react-hook-form";
import { PageBase } from "./PageBase";
import { ApiException } from "./ApiException";
import ErrorIcon from "@mui/icons-material/Error";

export const ErrorComponent: React.FC<{
  fullScreen?: boolean;
  error?: FieldErrors | ApiException;
  fontSize?: number;
}> = ({ fullScreen = false, error, fontSize }) => {
  return fullScreen ? (
    <PageBase>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        textAlign="center"
        sx={{ minHeight: "103vh" }}
      >
        {!error && (
          <Grid item width={"55%"}>
            <Alert
              severity="error"
              icon={<ErrorIcon color="inherit" fontSize="large" />}
            >
              <Typography fontSize={fontSize ?? 30}>
                Sorry, an error has occurred
              </Typography>
            </Alert>
          </Grid>
        )}
        {error instanceof Error && (
          <Grid item width={"55%"}>
            <Alert
              severity="error"
              icon={<ErrorIcon color="inherit" fontSize="large" />}
            >
              <Typography fontSize={fontSize ?? 30}>
                {error.message.includes("[object Object]")
                  ? "Sorry, an error has occurred"
                  : error.message}
              </Typography>
            </Alert>
          </Grid>
        )}
        {!(error instanceof ApiException) && error?.root?.message && (
          <Grid item width={"55%"}>
            <Alert
              severity="error"
              icon={<ErrorIcon color="inherit" fontSize="large" />}
            >
              <Typography fontSize={fontSize ?? 30}>
                {error.root.message}
              </Typography>
            </Alert>
          </Grid>
        )}
      </Grid>
    </PageBase>
  ) : (
    <Grid
      container
      justifyContent="center"
      alignItems="center"
      width="100%"
      textAlign="center"
    >
      {!error && (
        <Grid item width={"100%"}>
          <Alert severity="error">
            <Typography fontSize={fontSize ?? undefined}>
              Sorry, an error has occurred
            </Typography>
          </Alert>
        </Grid>
      )}
      {error instanceof Error && (
        <Grid item width={"100%"}>
          <Alert severity="error">
            <Typography fontSize={fontSize ?? undefined}>
              {error.message.includes("[object Object]")
                ? "Sorry, an error has occurred"
                : error.message}
            </Typography>
          </Alert>
        </Grid>
      )}
      {!(error instanceof ApiException) && error?.root?.message && (
        <Grid item width={"100%"}>
          <Alert severity="error">
            <Typography fontSize={fontSize ?? undefined}>
              {error.root.message}
            </Typography>
          </Alert>
        </Grid>
      )}
    </Grid>
  );
};
