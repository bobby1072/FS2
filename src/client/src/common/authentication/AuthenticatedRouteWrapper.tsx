import { useAuthentication } from "../contexts/AuthenticationContext";

export const AuthenticatedRouteWrapper: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const { isLoggedIn } = useAuthentication();
  if (!isLoggedIn) window.location.href = "/login";
  return <>{children}</>;
};
