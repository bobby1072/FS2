import React, { useCallback, useEffect } from "react";
import { Button } from "@mui/material";
import { useLocation } from "react-router-dom";
import { useAuthentication } from "./Authentication";

const Login: React.FC = () => {
  const { isLoggedIn, signIn } = useAuthentication();
  const location = useLocation();

  const doLogin = useCallback(async () => {
    await signIn({ state: { targetUrl: location.pathname } });
  }, [location.pathname, signIn]);

  useEffect(() => {
    !isLoggedIn && doLogin();
  }, [doLogin, isLoggedIn]);

  return (
    <Button onClick={doLogin}>Click here if you are not redirected.</Button>
  );
};

export default Login;
