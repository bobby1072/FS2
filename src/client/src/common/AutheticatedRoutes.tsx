import { AccountCircle } from "@mui/icons-material";
import { AllGroupDisplayPage } from "../pages/GroupPage";
import GroupsIcon from "@mui/icons-material/Groups";
import { AccountPage } from "../pages/AccountPage";
import { IndividualGroupPage } from "../pages/IndividualGroupPage";

export const AuthenticatedRoutes: {
  text: string;
  link: string;
  icon: () => JSX.Element;
  component: () => JSX.Element;
  showOnDrawer: boolean;
}[] = [
  {
    text: "Individual group page",
    link: "/Group/:id",
    showOnDrawer: false,
    icon: () => <></>,
    component: () => <IndividualGroupPage />,
  },
  {
    text: "Groups page",
    link: "/Groups",
    showOnDrawer: true,
    icon: () => <GroupsIcon />,
    component: () => <AllGroupDisplayPage />,
  },
  {
    text: "Account page",
    link: "/Account",
    showOnDrawer: false,
    component: () => <AccountPage />,
    icon: () => <AccountCircle />,
  },
];
