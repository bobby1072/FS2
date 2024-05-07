import { Grid, IconButton, Paper } from "@mui/material";
import { useState } from "react";
import ArrowBackIosIcon from "@mui/icons-material/ArrowBackIos";
import CancelIcon from "@mui/icons-material/Cancel";
export const MapControlBox: React.FC<{ children?: React.ReactNode }> = ({
  children,
}) => {
  const [mapOverlayOpen, setMapOverlayOpen] = useState<boolean>(true);
  return (
    <div style={{ position: "relative", width: "100%", height: "100%" }}>
      {mapOverlayOpen ? (
        <Paper
          sx={{
            position: "absolute",
            width: "20%",
            height: "40%",
            top: 10,
            right: 10,
            cursor: "default",
            zIndex: 5000,
          }}
        >
          <Grid
            container
            justifyContent="center"
            alignItems="center"
            padding={0.5}
          >
            <Grid
              item
              width="100%"
              sx={{ display: "flex", justifyContent: "flex-end" }}
            >
              <IconButton onClick={() => setMapOverlayOpen(false)} size="small">
                <CancelIcon />
              </IconButton>
            </Grid>
            <Grid item width="100%">
              {children && children}
            </Grid>
          </Grid>
        </Paper>
      ) : (
        <div onClick={() => setMapOverlayOpen(true)}>
          <Paper
            sx={{
              position: "absolute",
              width: "2%",
              height: "5%",
              top: 10,
              right: -5,
              cursor: "default",
              zIndex: 5000,
            }}
          >
            <Grid
              container
              height="100%"
              width="100%"
              justifyContent="center"
              alignItems="center"
              padding={0.1}
            >
              <Grid
                item
                width={"100%"}
                sx={{ display: "flex", justifyContent: "flex-start" }}
              >
                <IconButton size="small">
                  <ArrowBackIosIcon />
                </IconButton>
              </Grid>
            </Grid>
          </Paper>
        </div>
      )}
    </div>
  );
};
