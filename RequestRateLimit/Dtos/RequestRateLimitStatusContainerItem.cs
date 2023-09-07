namespace RequestRateLimit.Dtos;


public class RequestRateLimitStatusContainerItem
{
    public RequestRateLimitStatusPerTimeUnit unit { get; private set; }
    public int limit { get; private set; }
    public int amount { get; private set; }

    public RequestRateLimitStatusContainerItem(RequestRateLimitStatusPerTimeUnit unit, int limit, int amount)
    {
        this.unit = unit;
        this.limit = limit;
        this.amount = amount;
    }
}