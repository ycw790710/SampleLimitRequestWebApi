namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatusPerTimeUnitInfo
{
    public RequestRateLimitStatusPerTimeUnit PerTimeUnit { get; private set; }
    public string Name { get; private set; }

    public RequestRateLimitStatusPerTimeUnitInfo(RequestRateLimitStatusPerTimeUnit perTimeUnit, string name)
    {
        PerTimeUnit = perTimeUnit;
        Name = name;
    }
}