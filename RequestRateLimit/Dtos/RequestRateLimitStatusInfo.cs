namespace RequestRateLimit.Dtos;

public class RequestRateLimitStatusInfo
{
    public IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> containerTypeInfos { get; private set; }
    public IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> perUnitInfos { get; private set; }

    public RequestRateLimitStatusInfo(IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> containerTypeInfos, IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> perUnitInfos)
    {
        this.containerTypeInfos = containerTypeInfos;
        this.perUnitInfos = perUnitInfos;
    }
}