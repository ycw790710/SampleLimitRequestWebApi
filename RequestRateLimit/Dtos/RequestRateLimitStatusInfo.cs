namespace RequestRateLimit.Dtos;

public class RequestRateLimitStatusInfo
{
    public IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> typeInfos { get; private set; }
    public IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> unitUnitInfos { get; private set; }

    public RequestRateLimitStatusInfo(IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> typeInfos, IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> unitUnitInfos)
    {
        this.typeInfos = typeInfos;
        this.unitUnitInfos = unitUnitInfos;
    }
}