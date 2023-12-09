import { styled } from "@mui/material";
import { AppAndDraw } from "./AppBar/AppAndDraw";

const StyledPageDiv = styled("div")(() => ({
  height: "100vh",
  width: "100%",
  backgroundColor: "#F0F0F0",
}));

export const PageBase: React.FC<{ children?: React.ReactNode }> = ({
  children,
}) => {
  return (
    <StyledPageDiv>
      <AppAndDraw>{children}</AppAndDraw>
    </StyledPageDiv>
  );
};
