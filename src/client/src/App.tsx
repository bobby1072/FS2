import React, { ReactNode } from "react";

import { AuthenticationContextProvider } from "./common/contexts/AuthenticationContext";
import { useAppContext } from "./common/contexts/AppContext";

const { protocol, host } = window.location;

export const App: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { authorityClientId, authorityHost, scope } = useAppContext();

  return (
    <AuthenticationContextProvider
      clientRootHost={`${protocol}//${host}`}
      settings={{
        authority: authorityHost,
        client_id: authorityClientId,
        scope,
      }}
    >
      {children}
    </AuthenticationContextProvider>
  );
};
