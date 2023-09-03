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

import { exists, mapValues } from '../runtime';
import type { RequestRateLimitStatusContainerType } from './RequestRateLimitStatusContainerType';
import {
    RequestRateLimitStatusContainerTypeFromJSON,
    RequestRateLimitStatusContainerTypeFromJSONTyped,
    RequestRateLimitStatusContainerTypeToJSON,
} from './RequestRateLimitStatusContainerType';

/**
 * 
 * @export
 * @interface RequestRateLimitStatusContainerTypeInfo
 */
export interface RequestRateLimitStatusContainerTypeInfo {
    /**
     * 
     * @type {RequestRateLimitStatusContainerType}
     * @memberof RequestRateLimitStatusContainerTypeInfo
     */
    type?: RequestRateLimitStatusContainerType;
    /**
     * 
     * @type {string}
     * @memberof RequestRateLimitStatusContainerTypeInfo
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof RequestRateLimitStatusContainerTypeInfo
     */
    description?: string | null;
}

/**
 * Check if a given object implements the RequestRateLimitStatusContainerTypeInfo interface.
 */
export function instanceOfRequestRateLimitStatusContainerTypeInfo(value: object): boolean {
    let isInstance = true;

    return isInstance;
}

export function RequestRateLimitStatusContainerTypeInfoFromJSON(json: any): RequestRateLimitStatusContainerTypeInfo {
    return RequestRateLimitStatusContainerTypeInfoFromJSONTyped(json, false);
}

export function RequestRateLimitStatusContainerTypeInfoFromJSONTyped(json: any, ignoreDiscriminator: boolean): RequestRateLimitStatusContainerTypeInfo {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'type': !exists(json, 'type') ? undefined : RequestRateLimitStatusContainerTypeFromJSON(json['type']),
        'name': !exists(json, 'name') ? undefined : json['name'],
        'description': !exists(json, 'description') ? undefined : json['description'],
    };
}

export function RequestRateLimitStatusContainerTypeInfoToJSON(value?: RequestRateLimitStatusContainerTypeInfo | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'type': RequestRateLimitStatusContainerTypeToJSON(value.type),
        'name': value.name,
        'description': value.description,
    };
}

