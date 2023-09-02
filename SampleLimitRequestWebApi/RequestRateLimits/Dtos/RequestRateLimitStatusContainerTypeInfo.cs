namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;
public class RequestRateLimitStatusContainerTypeInfo
{
    public RequestRateLimitStatusContainerType Type { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    public RequestRateLimitStatusContainerTypeInfo(RequestRateLimitStatusContainerType type, string name, string description)
    {
        Type = type;
        Name = name;
        Description = description;
    }
}