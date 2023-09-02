# Org.OpenAPITools.Api.LimitGlobal3PerSecond5PerMinute4PerMinuteApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGet**](LimitGlobal3PerSecond5PerMinute4PerMinuteApi.md#apilimitglobal3persecond5perminute4perminutegetnormalget) | **GET** /api/LimitGlobal3PerSecond5PerMinute4PerMinute/Get_Normal |  |

<a id="apilimitglobal3persecond5perminute4perminutegetnormalget"></a>
# **ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGet**
> string ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGet (string? data = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new LimitGlobal3PerSecond5PerMinute4PerMinuteApi(config);
            var data = "data_example";  // string? |  (optional) 

            try
            {
                string result = apiInstance.ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGet(data);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LimitGlobal3PerSecond5PerMinute4PerMinuteApi.ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGetWithHttpInfo(data);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LimitGlobal3PerSecond5PerMinute4PerMinuteApi.ApiLimitGlobal3PerSecond5PerMinute4PerMinuteGetNormalGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **data** | **string?** |  | [optional]  |

### Return type

**string**

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

