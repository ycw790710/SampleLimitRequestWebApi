export enum ENV_CONFIG_ENV_TYPE {
  Test,
  Production,
  Development,
}
const ENV_CONFIG = {
  ENV_TYPE:
    ENV_CONFIG_ENV_TYPE[
      (process.env.REACT_APP_ENV_TYPE ??
        ENV_CONFIG_ENV_TYPE[
          ENV_CONFIG_ENV_TYPE.Test
        ]) as keyof typeof ENV_CONFIG_ENV_TYPE
    ],
  PORT: parseInt(process.env.REACT_APP_PORT ?? "3000"),
  DEMO: process.env.REACT_APP_DEMO === "0",
  DEMO_OR_DEV:
    process.env.REACT_APP_DEMO === "0" ||
    process.env.REACT_APP_ENV_TYPE ===
      ENV_CONFIG_ENV_TYPE[ENV_CONFIG_ENV_TYPE.Development],
  DEMO_OR_TEST:
    process.env.REACT_APP_DEMO === "0" ||
    process.env.REACT_APP_ENV_TYPE === undefined ||
    process.env.REACT_APP_ENV_TYPE ===
      ENV_CONFIG_ENV_TYPE[ENV_CONFIG_ENV_TYPE.Test],
  API_BASEURL: process.env.REACT_APP_API_BASEURL,
  MOBILE_WIDTH: 900,
};
export default ENV_CONFIG;
