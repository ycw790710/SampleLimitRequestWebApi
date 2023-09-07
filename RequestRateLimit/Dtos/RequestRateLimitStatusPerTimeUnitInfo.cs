namespace RequestRateLimit.Dtos;

public class RequestRateLimitStatusPerTimeUnitInfo
{
    public RequestRateLimitStatusPerTimeUnit unit { get; private set; }
    public string name { get; private set; }

    public RequestRateLimitStatusPerTimeUnitInfo(RequestRateLimitStatusPerTimeUnit unit, string name)
    {
        this.unit = unit;
        this.name = name;
    }
}