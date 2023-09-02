﻿namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatus
{
    public IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> ContainerTypeInfos { get; private set; }
    public IReadOnlyList<RequestRateLimitStatusPerTimeUnitInfo> PerUnitInfos { get; private set; }
    public DateTime UpdatedTime { get; private set; }
    public IReadOnlyDictionary<RequestRateLimitStatusContainerType, IReadOnlyList<RequestRateLimitStatusContainer>> ContainerTypesContainers { get; private set; }

    public RequestRateLimitStatus(IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> containerTypeTypeInfos,
        IReadOnlyList<RequestRateLimitStatusPerTimeUnitInfo> perUnitInfos,
        DateTime updatedTime,
        IReadOnlyDictionary<RequestRateLimitStatusContainerType, IReadOnlyList<RequestRateLimitStatusContainer>> containerTypesContainers)
    {
        ContainerTypeInfos = containerTypeTypeInfos;
        PerUnitInfos = perUnitInfos;
        UpdatedTime = updatedTime;
        ContainerTypesContainers = containerTypesContainers;
    }

}