import React, { ReactNode, createContext, useContext, useState } from "react";
import { ApiConfiguration } from "../apis/api-configuration";
import { CountApi } from "../apis/servers";

const initialConfiguration = new ApiConfiguration();
const ApiConfigurationContext = createContext({
  apiConfiguration: initialConfiguration,
  setApiConfiguration: (context: ApiConfiguration) => {},
});

// TODO: testCount
const testCount = () => {
  const countApi = new CountApi(initialConfiguration);
  setInterval(() => {
    try {
      countApi.apiCountGetNormalGet();
    } catch (error) {}
  }, 1);
};
testCount();

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
