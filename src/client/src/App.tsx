import React from "react";
import { ThemeProvider } from "@mui/material";
import { useClientConfigQuery } from "./common/queries/ClientConfigQuery";
import { fsTheme } from "./theme";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthWrapper } from "./common/login/Authentication";
import { LandingPage } from "./pages/LandingPage";
import Login from "./common/login/Login";
import { AuthenticatedRoute } from "./common/login/AuthenticatedRoute";
import { HomePage } from "./pages/HomePage";

const { protocol, host } = window.location;

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
            scope={data.authorityScope}
            silentRedirect={`${protocol}//${host}/oidc-silent-renew`}
          >
            <Routes>
              <Route path="/" element={<LandingPage />} />
              <Route path="/login" element={<Login />} />
              <Route
                path="/home"
                element={
                  <AuthenticatedRoute>
                    <HomePage />
                  </AuthenticatedRoute>
                }
              />
            </Routes>
          </AuthWrapper>
        )}
      </BrowserRouter>
    </ThemeProvider>
  );
};
