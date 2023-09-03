import React, { useEffect, useState } from "react";
import logo from "./logo.svg";
import "./App.css";
import { RequestRateLimitStatusApi } from "./apis/servers";
import { RequestRateLimitStatus } from "./apis/servers";
import { ApiConfiguration } from "./apis/api-configuration";

function App() {
  const requestRateLimitStatusApi = new RequestRateLimitStatusApi(
    new ApiConfiguration()
  );
  const [statusData, setStatusData] = useState<RequestRateLimitStatus>();
  useEffect(() => {
    const fetchData = async () => {
      try {
        await requestRateLimitStatusApi
          .apiRequestRateLimitStatusGetStatusPost()
          .then((data) => {
            setStatusData(data);
            console.log(data);
          });
      } catch (error) {
        console.error("Fail getting status:", error);
      }
    };

    const intervalId = setInterval(fetchData, 100);

    return () => {
      clearInterval(intervalId);
    };
  }, []);

  return (
    <div className={"body"}>
      <div className={"time"}>更新時間: {statusData?.updatedTime?.toLocaleString()}</div>
      <div className={"column-container column-title"}>
        {statusData?.containerTypeInfos?.map((n) => (
          <div className={"column text"}>{n.name}</div>
        ))}
      </div>
      <div className={"column-container"}>
        {statusData?.containerTypeInfos?.map((n) => (
          <div className={"column text"}>{n.description}</div>
        ))}
      </div>
      <div className={"column-container"}>
        {statusData?.containerTypeInfos?.map((n) => (
          <div className={"column"}>
            {statusData?.containerTypesContainers?.[
              n.type?.toString() ?? ""
            ].map((n) => (
              <div className={"row"}>
                <div className={"status-statuscontainer-item-id text"}>
                  ID [{n.key}]
                </div>
                <div>
                  {n.items?.map((n) => (
                    <div className={"status-statuscontainer-item-info text"}>
                      {n.capacity}/{n.capacity} [
                      {
                        statusData?.perUnitInfos?.[
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
