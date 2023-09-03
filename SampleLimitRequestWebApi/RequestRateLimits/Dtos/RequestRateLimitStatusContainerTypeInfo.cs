namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;
public class RequestRateLimitStatusContainerTypeInfo
{
    public RequestRateLimitStatusContainerType type { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }

    public RequestRateLimitStatusContainerTypeInfo(RequestRateLimitStatusContainerType type, string name, string description)
    {
        this.type = type;
        this.name = name;
        this.description = description;
    }
}