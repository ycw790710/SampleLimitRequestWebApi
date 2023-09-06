import { useCallback, useEffect, useMemo, useState } from "react";
import "./App.css";
import {
  RequestRateLimitStatusApi,
  RequestRateLimitStatusInfo,
} from "./apis/servers";
import { RequestRateLimitStatus } from "./apis/servers";
import { useApiConfiguration } from "./status-store/api-configuration-store";

const dateTimeOptions: Intl.DateTimeFormatOptions = {
  year: "numeric",
  month: "numeric",
  day: "numeric",
  hour: "2-digit",
  minute: "2-digit",
  second: "2-digit",
  fractionalSecondDigits: 3,
};

function App() {
  const { apiConfiguration, setApiConfiguration } = useApiConfiguration();

  const requestRateLimitStatusApi = useMemo(() => {
    return new RequestRateLimitStatusApi(apiConfiguration);
  }, [apiConfiguration]);

  const [reload, setReload] = useState(0);
  const [statusData, setStatusData] = useState<RequestRateLimitStatus>();
  const [statusDataError, setStatusDataError] = useState<string>();
  const [statusInfoData, setStatusInfoData] =
    useState<RequestRateLimitStatusInfo>();
  const [statusInfoDataError, setStatusInfoDataError] = useState<string>();

  const [selectedContainerTypes, setSelectedContainerTypes] = useState<
    string[]
  >([]);

  const refresh = () => {
    setReload((state) => state + 1);
  };

  const fetchData = useCallback(async () => {
    try {
      setStatusDataError(undefined);
      await requestRateLimitStatusApi
        .apiRequestRateLimitStatusGetStatusPost()
        .then((data) => {
          setStatusData(data);
        });
    } catch (error) {
      setStatusDataError("[ERROR Data]");
    }
  }, [requestRateLimitStatusApi]);

  useEffect(() => {
    const timeoutId = setTimeout(fetchData, 200);

    return () => {
      clearTimeout(timeoutId);
    };
  }, [statusData, fetchData, reload]);

  useEffect(() => {
    if (!statusInfoData) {
      const fetchInfoData = async () => {
        try {
          setStatusInfoDataError(undefined);
          await requestRateLimitStatusApi
            .apiRequestRateLimitStatusGetStatusInfoPost()
            .then((data) => {
              setStatusInfoData(data);
              setSelectedContainerTypes(
                data.containerTypeInfos?.map((n) => n.type?.toString() ?? "") ??
                  []
              );
              console.log("setStatusInfoData");
            });
        } catch (error) {
          setStatusInfoDataError("[ERROR InfoData]");
        }
      };

      fetchInfoData();
    }
  }, [statusInfoData, requestRateLimitStatusApi, reload]);

  const onChangeContainerType = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const dataValue = event.target.getAttribute("data-value") as string;

      setSelectedContainerTypes((state) => {
        const nextState: string[] = state.filter((n) => n !== dataValue);
        if (nextState.length === state.length) {
          nextState.push(dataValue);
        }
        console.log(nextState);

        return nextState;
      });
    },
    []
  );

  return (
    <div className={"body"}>
      <div className={"time"}>
        <button onClick={refresh} className={"margin1px"}>
          更新
        </button>
        更新時間: {statusData?.updatedTime?.toLocaleString([], dateTimeOptions)}
      </div>
      <div className={"error"}>
        {statusInfoDataError}
        {statusDataError}
      </div>
      <div className={"column-container"}>
        <div className={"column"}>
          {statusInfoData?.containerTypeInfos?.map((option) => (
            <div key={option.type}>
              <label
                htmlFor={"containerTypeInfo" + option.type?.toString() ?? ""}
              >
                {option.name ?? ""}
              </label>
              <input
                id={"containerTypeInfo" + option.type?.toString() ?? ""}
                type="checkbox"
                data-value={option.type?.toString() ?? ""}
                checked={selectedContainerTypes?.includes(
                  option.type?.toString() ?? ""
                )}
                onChange={onChangeContainerType}
              />
            </div>
          ))}
        </div>
      </div>
      <div className={"column-container column-title"}>
        {statusInfoData?.containerTypeInfos?.map(
          (n) =>
            selectedContainerTypes.includes(n.type?.toString() ?? "") && (
              <div key={n.type} className={"column text"}>
                {n.name}
              </div>
            )
        )}
      </div>
      <div className={"column-container"}>
        {statusInfoData?.containerTypeInfos?.map(
          (n) =>
            selectedContainerTypes.includes(n.type?.toString() ?? "") && (
              <div key={n.type} className={"column text"}>
                {n.description}
              </div>
            )
        )}
      </div>
      <div className={"column-container"}>
        {statusInfoData?.containerTypeInfos?.map(
          (n) =>
            selectedContainerTypes.includes(n.type?.toString() ?? "") && (
              <div key={n.type} className={"column"}>
                {statusData?.containerTypesContainers?.[
                  n.type?.toString() ?? ""
                ].map((n) => (
                  <div key={n.key} className={"row"}>
                    <div className={"status-statuscontainer-item-id text"}>
                      ID [{n.key}]
                    </div>
                    <div>
                      {n.items?.map((n) => (
                        <div
                          key={n.perTimeUnit}
                          className={"status-statuscontainer-item-info text"}
                        >
                          {n.capacity}/{n.limitTimes} [
                          {
                            statusInfoData?.perUnitInfos?.[
                              n.perTimeUnit?.toString() ?? ""
                            ].name
                          }
                          ]
                        </div>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            )
        )}
      </div>
    </div>
  );
}

export default App;
