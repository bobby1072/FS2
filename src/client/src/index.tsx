import React from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { App } from "./App";
import { QueryClient, QueryClientProvider } from "react-query";
import {
  Navigate,
  RouteObject,
  RouterProvider,
  createBrowserRouter,
} from "react-router-dom";
import { AuthenticatedRoutes } from "./common/AutheticatedRoutes";
import { UserManager } from "oidc-client-ts";
import { Loading } from "./common/Loading";
import { LandingPage } from "./pages/LandingPage";
import { AppContextProvider } from "./common/contexts/AppContext";
import { useAuthentication } from "./common/contexts/AuthenticationContext";
import { SignInCallback } from "./common/authentication/SignInCallback";
import { AuthenticatedRoute } from "./common/authentication/AuthenticatedRoute";
import { ThemeProvider } from "@mui/material";
import { fsTheme } from "./theme";
import { SnackbarProvider } from "notistack";
import { UserContextProvider } from "./common/contexts/UserContext";
document.title = "FS2";
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

const AppRoutes: RouteObject[] = [
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
  ...AuthenticatedRoutes.map((option) => ({
    path: option.link,
    element: (
      <Wrapper>
        <AuthenticatedRoute>
          <UserContextProvider>
            <option.component />
          </UserContextProvider>
        </AuthenticatedRoute>
      </Wrapper>
    ),
  })),
];

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: (count) => (count >= 1 ? false : true),
    },
  },
});
if (window.location.pathname === "/oidc-silent-renew") {
  new UserManager({} as any)
    .signinSilentCallback()
    .then()
    .catch((error: any) => {
      console.error(error);
    });
} else {
  const router = createBrowserRouter(AppRoutes);

  const container = document.getElementById("root");
  const root = createRoot(container!);

  root.render(
    <React.StrictMode>
      <ThemeProvider theme={fsTheme}>
        <QueryClientProvider client={queryClient}>
          <SnackbarProvider>
            <AppContextProvider>
              <RouterProvider
                router={router}
                fallbackElement={<Loading fullScreen={true} />}
              />
            </AppContextProvider>
          </SnackbarProvider>
        </QueryClientProvider>
      </ThemeProvider>
    </React.StrictMode>
  );
}
