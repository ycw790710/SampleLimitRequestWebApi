import { useCallback, useEffect, useMemo, useState } from "react";
import "./App.css";
import {
  RequestRateLimitStatusApi,
  RequestRateLimitStatusInfo,
} from "./apis/servers";
import { RequestRateLimitStatus } from "./apis/servers";
import { useApiConfiguration } from "./status-store/api-configuration-store";
import { HighlightText } from "./apis/components/HighlightText";

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

  const [containerTypeOptions, setContainerTypeOptions] = useState<
    { name: string; val: string }[]
  >([]);
  const [selectedContainerTypes, setSelectedContainerTypes] = useState<
    string[]
  >([]);

  const [statusContainerItemfilters, setStatusContainerItemfilters] = useState<{
    obj: { [key: string]: string };
  }>();

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
              const options =
                data.containerTypeInfos?.map((n) => ({
                  name: n.name ?? "",
                  val: n.type?.toString() ?? "",
                })) ?? [];
              setContainerTypeOptions(options);
              setSelectedContainerTypes(options.map((n) => n.val));

              const filters: any = {};
              options.forEach((n) => {
                filters[n.val] = [];
              });
              setStatusContainerItemfilters({ obj: filters });
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

  const onChangeStatusContainerItemfilters = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const val = event.target.value;
      const dataKey = event.target.getAttribute("data-key") as string;
      setStatusContainerItemfilters((state) => {
        if (state === undefined) return undefined;
        const netState = { ...state };
        netState.obj[dataKey] = val.toLowerCase();
        return netState;
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
          {containerTypeOptions.map((option) => (
            <div key={option.val}>
              <label htmlFor={"containerTypeInfoOption_" + option.val}>
                {option.name ?? ""}
              </label>
              <input
                id={"containerTypeInfoOption_" + option.val}
                type="checkbox"
                data-value={option.val}
                checked={selectedContainerTypes.includes(option.val)}
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
                <div className={"tooltip"}>
                  {n.name}
                  {n.description && (
                    <span className={"tooltiptext"}>{n.description}</span>
                  )}
                </div>
              </div>
            )
        )}
      </div>
      <div className={"column-container"}>
        {statusInfoData?.containerTypeInfos?.map(
          (n) =>
            selectedContainerTypes.includes(n.type?.toString() ?? "") && (
              <div key={n.type} className={"column text"}>
                <input
                  data-key={n.type?.toString() ?? ""}
                  placeholder={"過濾Key"}
                  value={
                    statusContainerItemfilters?.obj[n.type?.toString() ?? ""]
                  }
                  onChange={onChangeStatusContainerItemfilters}
                />
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
                ].map(
                  (n) =>
                    n.key
                      ?.toLowerCase()
                      .includes(
                        statusContainerItemfilters?.obj[
                          n.type?.toString() ?? ""
                        ] ?? ""
                      ) && (
                      <div key={n.key} className={"row"}>
                        <div
                          className={
                            "status-statuscontainer-item-id text tooltip"
                          }
                        >
                          <code className={"code"}>
                            <HighlightText
                              text={n.key}
                              keyword={
                                statusContainerItemfilters?.obj[
                                  n.type?.toString() ?? ""
                                ] ?? ""
                              }
                            />
                          </code>
                          <span className={"tooltiptext"}>Key</span>
                        </div>
                        <div>
                          {n.items?.map((n) => (
                            <div
                              key={n.perTimeUnit}
                              className={
                                "status-statuscontainer-item-info text"
                              }
                            >
                              <span>
                                (
                                {
                                  statusInfoData?.perUnitInfos?.[
                                    n.perTimeUnit?.toString() ?? ""
                                  ].name
                                }
                                )
                              </span>
                              <span className={"tooltip"}>
                                <code className={"code"}>
                                  {n.capacity}/{n.limitTimes}
                                </code>
                                <span className={"tooltiptext"}>
                                  使用數/限制
                                </span>
                              </span>
                            </div>
                          ))}
                        </div>
                      </div>
                    )
                )}
              </div>
            )
        )}
      </div>
    </div>
  );
}

export default App;
