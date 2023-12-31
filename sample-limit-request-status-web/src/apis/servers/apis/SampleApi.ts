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

export interface ApiSampleGetLimitGlobal1PreSecondGetRequest {
    data?: string;
}

export interface ApiSampleGetLimitGlobal3PreSecond5PerMinutesGetRequest {
    data?: string;
}

export interface ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetRequest {
    data?: string;
}

export interface ApiSampleGetLimitUser3PreSecondGetRequest {
    data?: string;
}

export interface ApiSampleGetNormalGetRequest {
    data?: string;
}

export interface ApiSamplePostLimit10BytesSizePostRequest {
    body?: string;
}

export interface ApiSamplePostNormalPostRequest {
    body?: string;
}

/**
 * 
 */
export class SampleApi extends runtime.BaseAPI {

    /**
     */
    async apiSampleGetLimitGlobal1PreSecondGetRaw(requestParameters: ApiSampleGetLimitGlobal1PreSecondGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        if (requestParameters.data !== undefined) {
            queryParameters['data'] = requestParameters.data;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/Sample/Get_LimitGlobal1PreSecond`,
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
    async apiSampleGetLimitGlobal1PreSecondGet(requestParameters: ApiSampleGetLimitGlobal1PreSecondGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSampleGetLimitGlobal1PreSecondGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiSampleGetLimitGlobal3PreSecond5PerMinutesGetRaw(requestParameters: ApiSampleGetLimitGlobal3PreSecond5PerMinutesGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        if (requestParameters.data !== undefined) {
            queryParameters['data'] = requestParameters.data;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/Sample/Get_LimitGlobal3PreSecond5PerMinutes`,
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
    async apiSampleGetLimitGlobal3PreSecond5PerMinutesGet(requestParameters: ApiSampleGetLimitGlobal3PreSecond5PerMinutesGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSampleGetLimitGlobal3PreSecond5PerMinutesGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiSampleGetLimitGlobal5PreSecondUser3PreSecondGetRaw(requestParameters: ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        if (requestParameters.data !== undefined) {
            queryParameters['data'] = requestParameters.data;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/Sample/Get_LimitGlobal5PreSecondUser3PreSecond`,
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
    async apiSampleGetLimitGlobal5PreSecondUser3PreSecondGet(requestParameters: ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSampleGetLimitGlobal5PreSecondUser3PreSecondGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiSampleGetLimitUser3PreSecondGetRaw(requestParameters: ApiSampleGetLimitUser3PreSecondGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        if (requestParameters.data !== undefined) {
            queryParameters['data'] = requestParameters.data;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/Sample/Get_LimitUser3PreSecond`,
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
    async apiSampleGetLimitUser3PreSecondGet(requestParameters: ApiSampleGetLimitUser3PreSecondGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSampleGetLimitUser3PreSecondGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiSampleGetNormalGetRaw(requestParameters: ApiSampleGetNormalGetRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        if (requestParameters.data !== undefined) {
            queryParameters['data'] = requestParameters.data;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/api/Sample/Get_Normal`,
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
    async apiSampleGetNormalGet(requestParameters: ApiSampleGetNormalGetRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSampleGetNormalGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiSamplePostLimit10BytesSizePostRaw(requestParameters: ApiSamplePostLimit10BytesSizePostRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/Sample/Post_Limit10BytesSize`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters.body as any,
        }, initOverrides);

        if (this.isJsonMime(response.headers.get('content-type'))) {
            return new runtime.JSONApiResponse<string>(response);
        } else {
            return new runtime.TextApiResponse(response) as any;
        }
    }

    /**
     */
    async apiSamplePostLimit10BytesSizePost(requestParameters: ApiSamplePostLimit10BytesSizePostRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSamplePostLimit10BytesSizePostRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async apiSamplePostNormalPostRaw(requestParameters: ApiSamplePostNormalPostRequest, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<runtime.ApiResponse<string>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/api/Sample/Post_Normal`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters.body as any,
        }, initOverrides);

        if (this.isJsonMime(response.headers.get('content-type'))) {
            return new runtime.JSONApiResponse<string>(response);
        } else {
            return new runtime.TextApiResponse(response) as any;
        }
    }

    /**
     */
    async apiSamplePostNormalPost(requestParameters: ApiSamplePostNormalPostRequest = {}, initOverrides?: RequestInit | runtime.InitOverrideFunction): Promise<string> {
        const response = await this.apiSamplePostNormalPostRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
