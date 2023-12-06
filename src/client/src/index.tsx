import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import { App } from "./App";
import { QueryClient, QueryClientProvider } from "react-query";
document.title = "FS2";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: (count) => (count >= 3 ? false : true),
    },
  },
});
const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement
);
root.render(
  <React.StrictMode>
    <QueryClientProvider {...{ client: queryClient }}>
      <App />
    </QueryClientProvider>
  </React.StrictMode>
);
