namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatusPerTimeUnitInfo
{
    public RequestRateLimitStatusPerTimeUnit perTimeUnit { get; private set; }
    public string name { get; private set; }

    public RequestRateLimitStatusPerTimeUnitInfo(RequestRateLimitStatusPerTimeUnit perTimeUnit, string name)
    {
        this.perTimeUnit = perTimeUnit;
        this.name = name;
    }
}