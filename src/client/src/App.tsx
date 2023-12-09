import React, { ReactNode } from "react";
import { ThemeProvider } from "@mui/material";
import { useClientConfigQuery } from "./common/queries/ClientConfigQuery";
import { fsTheme } from "./theme";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthWrapper } from "./common/login/Authentication";
import { LandingPage } from "./pages/LandingPage";
import Login from "./common/login/Login";
import { AuthenticatedRoute } from "./common/login/AuthenticatedRoute";
import { AccountPage } from "./pages/AccountPage";
import { UserContextProvider } from "./common/UserContext";

const { protocol, host } = window.location;

const DefaultWrappers: React.FC<{ children: ReactNode }> = ({ children }) => {
  return (
    <AuthenticatedRoute>
      <UserContextProvider>{children}</UserContextProvider>
    </AuthenticatedRoute>
  );
};

export const App: React.FC = () => {
  const { data } = useClientConfigQuery();
  return (
    <ThemeProvider theme={fsTheme}>
      <BrowserRouter>
        {data && (
          <AuthWrapper
            authorityHost={data.authorityHost}
            clientId={data.authorityClientId}
            redirectUri={`${protocol}//${host}/oidc-signin`}
            scope={"email openid profile"}
            silentRedirect={`${protocol}//${host}/oidc-silent-renew`}
          >
            <Routes>
              <Route path="/" element={<LandingPage />} />
              <Route path="/Login" element={<Login />} />
              <Route
                path="/Account"
                element={
                  <DefaultWrappers>
                    <AccountPage />
                  </DefaultWrappers>
                }
              />
            </Routes>
          </AuthWrapper>
        )}
      </BrowserRouter>
    </ThemeProvider>
  );
};
