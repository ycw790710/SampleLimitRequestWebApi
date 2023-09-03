using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.CodeExtensions;

public class HttpResponseMessageResult : ActionResult
{
    private readonly HttpResponseMessage _response;

    public HttpResponseMessageResult(HttpResponseMessage response)
    {
        _response = response;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var httpContext = context.HttpContext;
        var response = httpContext.Response;

        response.StatusCode = (int)_response.StatusCode;

        foreach (var header in _response.Headers)
        {
            response.Headers[header.Key] = header.Value.ToArray();
        }

        if (_response.Content != null)
        {
            var content = await _response.Content.ReadAsByteArrayAsync();
            await response.Body.WriteAsync(content, 0, content.Length);
        }
    }
}