import { AccountCircle } from "@mui/icons-material";
import { AllGroupDisplayPage } from "../pages/GroupsPage";
import GroupsIcon from "@mui/icons-material/Groups";
import { IndividualAccountPage } from "../pages/AccountPage";
import { IndividualGroupPage } from "../pages/IndividualGroupPage";
import { IndividualCatchPage } from "../pages/IndividualCatchPage";

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
    text: "Individual group catch page",
    link: "/GroupCatch/:id",
    showOnDrawer: false,
    icon: () => <></>,
    component: () => <IndividualCatchPage />,
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
    link: "/Account/:id",
    showOnDrawer: false,
    component: () => <IndividualAccountPage />,
    icon: () => <AccountCircle />,
  },
];
