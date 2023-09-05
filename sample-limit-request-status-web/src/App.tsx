import { useCallback, useEffect, useMemo, useState } from "react";
import "./App.css";
import {
  RequestRateLimitStatusApi,
  RequestRateLimitStatusInfo,
  CountApi,
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

  // const countApi = useMemo(() => {
  //   return new CountApi(apiConfiguration);
  // }, [apiConfiguration]);
  // const [countData, setCountData] = useState(0);

  // useEffect(() => {
  //   const intervalId = setInterval(async () => {
  //     try {
  //       setCountData(-1);
  //       await countApi.apiCountGetNormalGet().then((data) => {
  //         setCountData(data);
  //       });
  //     } catch (error) {
  //       setCountData(-1);
  //     }
  //   }, 50);

  //   return () => {
  //     clearInterval(intervalId);
  //   };
  // }, []);

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
    const timeoutId = setTimeout(fetchData, 100);

    return () => {
      clearTimeout(timeoutId);
    };
  }, [statusData, fetchData, reload]);

  // useEffect(() => {
  //   const intervalId = setInterval(fetchData, 50);

  //   return () => {
  //     clearInterval(intervalId);
  //   };
  // }, []);

  useEffect(() => {
    if (!statusInfoData) {
      const fetchInfoData = async () => {
        try {
          setStatusInfoDataError(undefined);
          await requestRateLimitStatusApi
            .apiRequestRateLimitStatusGetStatusInfoPost()
            .then((data) => {
              setStatusInfoData(data);
            });
        } catch (error) {
          setStatusInfoDataError("[ERROR InfoData]");
        }
      };

      fetchInfoData();
    }
  }, [statusInfoData, requestRateLimitStatusApi, reload]);

  return (
    <div className={"body"}>
      <div className={"time"}>
        <button onClick={refresh}>更新</button>
        更新時間: {statusData?.updatedTime?.toLocaleString([], dateTimeOptions)}
      </div>
      <div>
        {statusInfoDataError}
        {statusDataError}
      </div>
      <div className={"column-container column-title"}>
        {statusInfoData?.containerTypeInfos?.map((n) => (
          <div key={n.type} className={"column text"}>
            {n.name}
          </div>
        ))}
      </div>
      <div className={"column-container"}>
        {statusInfoData?.containerTypeInfos?.map((n) => (
          <div key={n.type} className={"column text"}>
            {n.description}
          </div>
        ))}
      </div>
      <div className={"column-container"}>
        {statusInfoData?.containerTypeInfos?.map((n) => (
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
        ))}
      </div>
    </div>
  );
}

export default App;
