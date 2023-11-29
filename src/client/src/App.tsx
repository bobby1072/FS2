import React from "react";
import logo from "./logo.svg";
import "./App.css";
import { ThemeProvider } from "@mui/material";
import { useClientConfigQuery } from "./common/queries/ClientConfigQuery";
import { fsTheme } from "./theme";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthWrapper } from "./common/login/Authentication";

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
              <Route path="/" element={<HomePage />} />
            </Routes>
          </AuthWrapper>
        )}
      </BrowserRouter>
    </ThemeProvider>
  );
};

function HomePage() {
  const { data, error, isLoading } = useClientConfigQuery();
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}
