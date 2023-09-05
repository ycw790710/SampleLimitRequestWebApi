import { Configuration } from "./servers";

import ENV_CONFIG from "../ENV_CONFIG";

export class ApiConfiguration extends Configuration {
  constructor() {
    super({
      basePath: ENV_CONFIG.API_BASEURL,
    });
  }
}
