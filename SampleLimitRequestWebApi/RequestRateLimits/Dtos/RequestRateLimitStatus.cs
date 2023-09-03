namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatus
{
    public IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> containerTypeInfos { get; private set; }
    public IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> perUnitInfos { get; private set; }
    public DateTime updatedTime { get; private set; }
    public IReadOnlyDictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>> containerTypesContainers { get; private set; }

    public RequestRateLimitStatus(IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> containerTypeTypeInfos,
        IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> perUnitInfos,
        DateTime updatedTime,
        IReadOnlyDictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>> containerTypesContainers)
    {
        containerTypeInfos = containerTypeTypeInfos;
        this.perUnitInfos = perUnitInfos;
        this.updatedTime = updatedTime;
        this.containerTypesContainers = containerTypesContainers;
    }

}