/* tslint:disable */
/* eslint-disable */
/**
 * SampleLimitRequestWebApi
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';

/**
 * 
 */
export class LimitIpController3s5m100hApi extends runtime.BaseAPI {

    /**
     */
    async apiLimitIpController3s5m100hGetGetRaw(initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/LimitIpController3s5m100h/Get`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        if (this.isJsonMime(response.headers.get('content-type'))) {
            return new runtime.JSONApiResponse<string>(response);
        } else {
            return new runtime.TextApiResponse(response) as any;
        }
    }

    /**
     */
    async apiLimitIpController3s5m100hGetGet(initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiLimitIpController3s5m100hGetGetRaw(initOverrides);
        return await response.value();
    }

}
