import {
  AuthProvider,
  AuthProviderProps,
  useAuth as useOidcAuth,
  User,
  UserManager,
} from "oidc-react";
import React, { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { SigninRedirectArgs } from "oidc-client-ts";
import { Loading } from "../Loading";
import Settings from "../../settings";

interface Authentication {
  bearerToken?: string;
  isLoggedIn: boolean;
  profile?: User["profile"];
  signIn: (args?: SigninRedirectArgs) => Promise<void>;
  logout: () => void;
}

interface Props {
  authorityHost: string;
  clientId: string;
  children: React.ReactNode;
}

export const AuthWrapper: React.FC<Props> = ({
  children,
  authorityHost,
  clientId,
}) => {
  const push = useNavigate();
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  const [userManager] = useState<UserManager>(
    new UserManager({
      ...Settings.oidcSettings,
      authority: authorityHost,
      client_id: clientId,
    })
  );

  useEffect(() => {
    userManager.clearStaleState();
  }, [userManager]);

  const onSessionEnd = useCallback(() => {
    setIsLoggedIn(false);
    userManager.signoutRedirect();
  }, [userManager]);

  useEffect(() => {
    userManager.events.addAccessTokenExpired(onSessionEnd);

    return () => {
      userManager.events.removeAccessTokenExpired(onSessionEnd);
    };
  }, [userManager, onSessionEnd]);

  const oidcConfig: AuthProviderProps = {
    userManager,
    autoSignIn: false,
    onSignIn: async (user: User | null) => {
      setIsLoggedIn(user !== null && user !== undefined);

      if (
        user !== null &&
        user !== undefined &&
        (user.state as any)?.targetUrl !== undefined &&
        window.location.pathname !== (user.state as any).targetUrl
      ) {
        push((user.state as any).targetUrl);
      }
    },
  };

  const urlParams = new URLSearchParams(window.location.search);
  const oidcState = urlParams.get("code");
  const loading = !isLoggedIn && oidcState !== null;

  return (
    <AuthProvider {...oidcConfig}>
      {loading ? <Loading fullScreen /> : children}
    </AuthProvider>
  );
};

export const useAuthentication = (): Authentication => {
  const oidcAuth = useOidcAuth();
  const push = useNavigate();

  return {
    bearerToken:
      oidcAuth?.userData?.access_token.split("Bearer ").length === 2
        ? oidcAuth?.userData?.access_token
        : `Bearer ${oidcAuth?.userData?.access_token}`,
    isLoggedIn: oidcAuth?.userData !== undefined && oidcAuth?.userData !== null,
    profile: oidcAuth?.userData?.profile,
    signIn: oidcAuth!.signIn,
    logout: async () => {
      await oidcAuth!.signOutRedirect();
      push("/Login");
    },
  };
};

export const SignoutCallback: React.FC = () => {
  const { logout } = useAuthentication();

  useEffect(() => {
    const handleCallback = async () => {
      await logout();
    };
    handleCallback();
  }, [logout]);

  return null;
};
