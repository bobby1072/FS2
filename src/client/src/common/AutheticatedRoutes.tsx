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
  constantRefreshCurrentUser: boolean;
}[] = [
  {
    text: "Individual group page",
    link: "/Group/:id",
    showOnDrawer: false,
    icon: () => <></>,
    component: () => <IndividualGroupPage />,
    constantRefreshCurrentUser: false
  },
  {
    text: "Groups page",
    link: "/Groups",
    showOnDrawer: true,
    icon: () => <GroupsIcon />,
    component: () => <AllGroupDisplayPage />,
    constantRefreshCurrentUser: false
  },
  {
    text: "Account page",
    link: "/Account",
    showOnDrawer: false,
    component: () => <AccountPage />,
    icon: () => <AccountCircle />,
    constantRefreshCurrentUser: true
  },
];
