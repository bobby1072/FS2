import { AllGroupDisplayPage } from "../pages/GroupPage";
import GroupsIcon from "@mui/icons-material/Groups";

export const AuthenticatedRoutes: {
  text: string;
  link: string;
  icon: () => JSX.Element;
  component: () => JSX.Element;
  showOnDrawer: boolean;
}[] = [
  {
    text: "Groups page",
    link: "/Groups",
    showOnDrawer: true,
    icon: () => <GroupsIcon />,
    component: () => <AllGroupDisplayPage />,
  },
];
