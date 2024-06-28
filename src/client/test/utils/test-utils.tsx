import { ReactElement } from "react";
import { QueryClient, QueryClientProvider } from "react-query";
import { RenderOptions, render } from "@testing-library/react";
import React from "react";
const queryClient = new QueryClient();
interface IWrapper {
  children: any;
}
const customRender = (ui: ReactElement, options?: RenderOptions) => {
  let wrapper: React.FC<IWrapper>;
  if (options?.wrapper) {
    const Wrapper = options.wrapper;
    wrapper = ({ children }) => (
      <Wrapper>
        <QueryClientProvider client={queryClient}>
          {children}
        </QueryClientProvider>
      </Wrapper>
    );
  } else {
    wrapper = ({ children }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );
  }
  return render(ui, { ...options, wrapper });
};
export * from "@testing-library/react";
export { customRender as render };
