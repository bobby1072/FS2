import { createTheme } from "@mui/material";

export const fsTheme = createTheme({
  palette: {
    primary: {
      light: "#8dd7ff", // Lightened shade of sea blue
      main: "#2196f3", // Sea blue as the main color
      dark: "#1565c0", // Darkened shade of sea blue
      contrastText: "#ffffff", // White text for better contrast
    },
    secondary: {
      light: "#ff4081", // Lightened shade of secondary color
      main: "#e91e63", // Main secondary color
      dark: "#ad1457", // Darkened shade of secondary color
      contrastText: "#ffffff", // White text for better contrast
    },
  },
});
