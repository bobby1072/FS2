import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react-swc";
import svgrPlugin from "vite-plugin-svgr";

export default defineConfig({
  server: {
    port: 3000,
  },
  build: {
    outDir: "build",
  },
  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: "src/setupTests.tsx",
    testTimeout: 15000,
    coverage: {
      reportsDirectory: "./reports",
      reporter: ["json", "html"],
      include: ["src/**/*"],
      exclude: [],
    },
  },
  plugins: [react(), svgrPlugin()],
  define: {
    __DEV__: (process.env.NODE_ENV === "development").toString(),
  },
});
