import * as React from "react";
import { useAuthentication } from "./Authentication";
import Login from "./Login";

interface RequireAuthProps {
  redirectTo?: string;
  children?: React.ReactNode;
}

export const AuthenticatedRoute: React.FC<RequireAuthProps> = ({
  redirectTo,
  children,
}) => {
  const { isLoggedIn } = useAuthentication();

  if (isLoggedIn) {
    return <>{children}</>;
  } else {
    return <Login />;
  }
};
