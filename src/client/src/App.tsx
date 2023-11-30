import React from "react";
import { ThemeProvider } from "@mui/material";
import { useClientConfigQuery } from "./common/queries/ClientConfigQuery";
import { fsTheme } from "./theme";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthWrapper } from "./common/login/Authentication";
import { LandingPage } from "./pages/LandingPage";

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
              <Route
                path="/"
                element={
                  <LandingPage
                    redirectUri={`${protocol}//${host}/oidc-signin`}
                  />
                }
              />
            </Routes>
          </AuthWrapper>
        )}
      </BrowserRouter>
    </ThemeProvider>
  );
};
