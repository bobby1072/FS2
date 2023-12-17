import React, { ReactNode, useEffect, useState } from "react";
import { ThemeProvider } from "@mui/material";
import { useClientConfigQuery } from "./common/queries/ClientConfigQuery";
import { fsTheme } from "./theme";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AuthWrapper } from "./common/login/Authentication";
import { LandingPage } from "./pages/LandingPage";
import Login from "./common/login/Login";
import { AuthenticatedRoute } from "./common/login/AuthenticatedRoute";
import { AccountPage } from "./pages/AccountPage";
import { UserContextProvider } from "./common/UserContext";
import { Loading } from "./common/Loading";
import { AuthenticatedRoutes } from "./common/AutheticatedRoutes";

const DefaultWrappers: React.FC<{ children: ReactNode }> = ({ children }) => {
  return (
    <AuthenticatedRoute>
      <UserContextProvider>{children}</UserContextProvider>
    </AuthenticatedRoute>
  );
};

export const App: React.FC = () => {
  const [authoritySettings, setAuthoritySettings] = useState<{
    host: string;
    clientId: string;
  } | null>(null);
  const { data } = useClientConfigQuery();
  useEffect(() => {
    if (data) {
      setAuthoritySettings({
        clientId: data?.authorityClientId,
        host: data?.authorityHost,
      });
    }
  }, [data]);
  if (!authoritySettings) return <Loading fullScreen />;

  return (
    <ThemeProvider theme={fsTheme}>
      <BrowserRouter>
        {data && (
          <AuthWrapper
            authorityHost={authoritySettings.host}
            clientId={authoritySettings.clientId}
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
              {AuthenticatedRoutes.map((x) => (
                <Route
                  key={x.link}
                  path={x.link}
                  element={
                    <DefaultWrappers>
                      <x.component />
                    </DefaultWrappers>
                  }
                />
              ))}
              <Route path="/Home" element={<Navigate to="/Groups" />} />
            </Routes>
          </AuthWrapper>
        )}
      </BrowserRouter>
    </ThemeProvider>
  );
};
