# Org.OpenAPITools.Api.RequestRateLimitStatusApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiRequestRateLimitStatusGetStatusInfoPost**](RequestRateLimitStatusApi.md#apirequestratelimitstatusgetstatusinfopost) | **POST** /api/RequestRateLimitStatus/GetStatusInfo |  |
| [**ApiRequestRateLimitStatusGetStatusPost**](RequestRateLimitStatusApi.md#apirequestratelimitstatusgetstatuspost) | **POST** /api/RequestRateLimitStatus/GetStatus |  |

<a id="apirequestratelimitstatusgetstatusinfopost"></a>
# **ApiRequestRateLimitStatusGetStatusInfoPost**
> RequestRateLimitStatusInfo ApiRequestRateLimitStatusGetStatusInfoPost ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRequestRateLimitStatusGetStatusInfoPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestRateLimitStatusApi(config);

            try
            {
                RequestRateLimitStatusInfo result = apiInstance.ApiRequestRateLimitStatusGetStatusInfoPost();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestRateLimitStatusApi.ApiRequestRateLimitStatusGetStatusInfoPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRequestRateLimitStatusGetStatusInfoPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RequestRateLimitStatusInfo> response = apiInstance.ApiRequestRateLimitStatusGetStatusInfoPostWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RequestRateLimitStatusApi.ApiRequestRateLimitStatusGetStatusInfoPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**RequestRateLimitStatusInfo**](RequestRateLimitStatusInfo.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apirequestratelimitstatusgetstatuspost"></a>
# **ApiRequestRateLimitStatusGetStatusPost**
> RequestRateLimitStatus ApiRequestRateLimitStatusGetStatusPost ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiRequestRateLimitStatusGetStatusPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new RequestRateLimitStatusApi(config);

            try
            {
                RequestRateLimitStatus result = apiInstance.ApiRequestRateLimitStatusGetStatusPost();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RequestRateLimitStatusApi.ApiRequestRateLimitStatusGetStatusPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRequestRateLimitStatusGetStatusPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<RequestRateLimitStatus> response = apiInstance.ApiRequestRateLimitStatusGetStatusPostWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RequestRateLimitStatusApi.ApiRequestRateLimitStatusGetStatusPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**RequestRateLimitStatus**](RequestRateLimitStatus.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

