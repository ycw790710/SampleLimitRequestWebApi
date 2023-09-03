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
import type { RequestRateLimitStatusContainerItem } from './RequestRateLimitStatusContainerItem';
import {
    RequestRateLimitStatusContainerItemFromJSON,
    RequestRateLimitStatusContainerItemFromJSONTyped,
    RequestRateLimitStatusContainerItemToJSON,
} from './RequestRateLimitStatusContainerItem';
import type { RequestRateLimitStatusContainerType } from './RequestRateLimitStatusContainerType';
import {
    RequestRateLimitStatusContainerTypeFromJSON,
    RequestRateLimitStatusContainerTypeFromJSONTyped,
    RequestRateLimitStatusContainerTypeToJSON,
} from './RequestRateLimitStatusContainerType';

/**
 * 
 * @export
 * @interface RequestRateLimitStatusContainer
 */
export interface RequestRateLimitStatusContainer {
    /**
     * 
     * @type {string}
     * @memberof RequestRateLimitStatusContainer
     */
    key?: string | null;
    /**
     * 
     * @type {Date}
     * @memberof RequestRateLimitStatusContainer
     */
    updatedTime?: Date;
    /**
     * 
     * @type {RequestRateLimitStatusContainerType}
     * @memberof RequestRateLimitStatusContainer
     */
    type?: RequestRateLimitStatusContainerType;
    /**
     * 
     * @type {Array<RequestRateLimitStatusContainerItem>}
     * @memberof RequestRateLimitStatusContainer
     */
    items?: Array<RequestRateLimitStatusContainerItem> | null;
}

/**
 * Check if a given object implements the RequestRateLimitStatusContainer interface.
 */
export function instanceOfRequestRateLimitStatusContainer(value: object): boolean {
    let isInstance = true;

    return isInstance;
}

export function RequestRateLimitStatusContainerFromJSON(json: any): RequestRateLimitStatusContainer {
    return RequestRateLimitStatusContainerFromJSONTyped(json, false);
}

export function RequestRateLimitStatusContainerFromJSONTyped(json: any, ignoreDiscriminator: boolean): RequestRateLimitStatusContainer {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'key': !exists(json, 'key') ? undefined : json['key'],
        'updatedTime': !exists(json, 'updatedTime') ? undefined : (new Date(json['updatedTime'])),
        'type': !exists(json, 'type') ? undefined : RequestRateLimitStatusContainerTypeFromJSON(json['type']),
        'items': !exists(json, 'items') ? undefined : (json['items'] === null ? null : (json['items'] as Array<any>).map(RequestRateLimitStatusContainerItemFromJSON)),
    };
}

export function RequestRateLimitStatusContainerToJSON(value?: RequestRateLimitStatusContainer | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'key': value.key,
        'updatedTime': value.updatedTime === undefined ? undefined : (value.updatedTime.toISOString()),
        'type': RequestRateLimitStatusContainerTypeToJSON(value.type),
        'items': value.items === undefined ? undefined : (value.items === null ? null : (value.items as Array<any>).map(RequestRateLimitStatusContainerItemToJSON)),
    };
}

