namespace RequestRateLimit.Dtos;

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