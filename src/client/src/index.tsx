import React from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { App } from "./App";
import { QueryClient, QueryClientProvider } from "react-query";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { AuthenticatedRoutes } from "./common/AutheticatedRoutes";
import { UserManager } from "oidc-client-ts";
import { LandingPage } from "./pages/LandingPage";
import { AppContextProvider } from "./common/contexts/AppContext";
import { useAuthentication } from "./common/contexts/AuthenticationContext";
import { SignInCallback } from "./common/authentication/SignInCallback";
import { AuthenticatedRouteWrapper } from "./common/authentication/AuthenticatedRouteWrapper";
import { ThemeProvider } from "@mui/material";
import { fsTheme } from "./theme";
import { SnackbarProvider } from "notistack";
import { UserContextProvider } from "./common/contexts/UserContext";
import { PermissionContextProvider } from "./common/contexts/AbilitiesContext";
import { LocalizationProvider } from "@mui/x-date-pickers";
const FallbackRoute: React.FC = () => {
  const { isLoggedIn } = useAuthentication();
  return isLoggedIn ? (
    <Navigate to={`/oidc-signin`} />
  ) : (
    <Navigate to={`/login`} />
  );
};

const Wrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <App>{children}</App>
);

const AppRoutes = [
  {
    path: "/",
    element: (
      <Wrapper>
        <FallbackRoute />
      </Wrapper>
    ),
  },
  {
    path: "/login",
    element: (
      <Wrapper>
        <LandingPage />
      </Wrapper>
    ),
  },
  {
    path: "/Home",
    element: <Navigate to="/Groups" />,
  },
  {
    path: "/oidc-signin",
    element: (
      <Wrapper>
        <SignInCallback />
      </Wrapper>
    ),
  },
  ...AuthenticatedRoutes?.map(({ link, component }) => ({
    path: link,
    element: (
      <Wrapper>
        <AuthenticatedRouteWrapper>
          <UserContextProvider>
            <PermissionContextProvider>{component()}</PermissionContextProvider>
          </UserContextProvider>
        </AuthenticatedRouteWrapper>
      </Wrapper>
    ),
  })),
];
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: (count) => (count >= 2 ? false : true),
    },
  },
});
if (window.location.pathname === "/oidc-silent-renew") {
  new UserManager({} as any).signinSilentCallback().catch((error: any) => {
    console.error(error);
  });
} else {
  const container = document.getElementById("root");
  const root = createRoot(container!);

  root.render(
    <React.StrictMode>
      <ThemeProvider theme={fsTheme}>
        <LocalizationProvider dateAdapter={AdapterDateFns}>
          <QueryClientProvider client={queryClient}>
            <SnackbarProvider>
              <AppContextProvider>
                <BrowserRouter>
                  <Routes>
                    {AppRoutes?.map((r) => (
                      <Route element={r.element} path={r.path} />
                    ))}
                  </Routes>
                </BrowserRouter>
              </AppContextProvider>
            </SnackbarProvider>
          </QueryClientProvider>
        </LocalizationProvider>
      </ThemeProvider>
    </React.StrictMode>
  );
}
