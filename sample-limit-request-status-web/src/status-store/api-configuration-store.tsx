import React, { ReactNode, createContext, useContext, useState } from "react";
import { ApiConfiguration } from "../apis/api-configuration";

const initialConfiguration = new ApiConfiguration();
const ApiConfigurationContext = createContext({
  apiConfiguration: initialConfiguration,
  setApiConfiguration: (context: ApiConfiguration) => {},
});

export function ApiConfigurationProvider({
  children,
}: {
  children: ReactNode;
}) {
  const [apiConfiguration, setApiConfiguration] =
    useState<ApiConfiguration>(initialConfiguration);

  return (
    <ApiConfigurationContext.Provider
      value={{ apiConfiguration, setApiConfiguration }}
    >
      {children}
    </ApiConfigurationContext.Provider>
  );
}

export function useApiConfiguration() {
  return useContext(ApiConfigurationContext);
}
