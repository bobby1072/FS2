import { styled } from "@mui/material";
import { AppAndDraw } from "./AppBar/AppAndDraw";

const StyledPageDiv = styled("div")(({ theme }) => ({
  height: "100vh",
  width: "100%",
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
