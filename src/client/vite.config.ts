import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import svgrPlugin from "vite-plugin-svgr";

export default defineConfig({
  server: {
    port: 3000,
  },
  build: {
    outDir: "build",
  },
  plugins: [react(), svgrPlugin()],
  define: {
    __DEV__: (process.env.NODE_ENV === "development").toString(),
  },
});
