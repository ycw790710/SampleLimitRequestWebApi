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


/**
 * 
 * @export
 */
export const RequestRateLimitStatusContainerType = {
    NUMBER_0: 0,
    NUMBER_1: 1,
    NUMBER_2: 2,
    NUMBER_3: 3
} as const;
export type RequestRateLimitStatusContainerType = typeof RequestRateLimitStatusContainerType[keyof typeof RequestRateLimitStatusContainerType];


export function RequestRateLimitStatusContainerTypeFromJSON(json: any): RequestRateLimitStatusContainerType {
    return RequestRateLimitStatusContainerTypeFromJSONTyped(json, false);
}

export function RequestRateLimitStatusContainerTypeFromJSONTyped(json: any, ignoreDiscriminator: boolean): RequestRateLimitStatusContainerType {
    return json as RequestRateLimitStatusContainerType;
}

export function RequestRateLimitStatusContainerTypeToJSON(value?: RequestRateLimitStatusContainerType | null): any {
    return value as any;
}

