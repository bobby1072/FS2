import "@testing-library/jest-dom";
import ResizeObserver from "resize-observer-polyfill";
import { vi } from "vitest";
window.ResizeObserver = ResizeObserver;

vi.mock("react-router-dom", async () => ({
  ...(await vi.importActual<any>("react-router-dom")),
  useParams: vi.fn(),
  useNavigate: vi.fn().mockReturnValue(vi.fn()),
  Link: ({ children, to }: any) => {
    return <span aria-label={to}>{children}</span>;
  },
}));

vi.mock("./common/contexts/AbilitiesContext", async () => {
  return {
    ...(await vi.importActual<any>("./common/contexts/AbilitiesContext")),

    useCurrentPermissionManager: vi.fn().mockReturnValue({
      permissionManager: {
        Can: vi.fn().mockReturnValue(true),
      },
    }),
  };
});
vi.mock("./common/contexts/AuthenticationContext", async () => {
  return {
    ...(await vi.importActual<any>("./common/contexts/AuthenticationContext")),
    useAuthentication: vi.fn().mockReturnValue({ isLoggedIn: true }),
  };
});

vi.mock("./common/contexts/UserContext", async () => ({
  ...(await vi.importActual<any>("./common/contexts/UserContext")),
  useCurrentUser: vi.fn().mockReturnValue({ id: "1" }),
}));
