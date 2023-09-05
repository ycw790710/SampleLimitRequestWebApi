using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace SampleLimitRequestWebApi.Controllers;

public class CountController : DefaultControllerBase
{
    static readonly object lockObj = new();
    static readonly ConcurrentDictionary<TimeSpan, TimeSpan> keys = new();
    static readonly ConcurrentQueue<TimeSpan> waitExpireds = new();

    /// <summary>
    /// 對照組
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<int>> Get_Normal([FromQuery] string? data)
    {
        RemoveExpired();

        var key = GlobalTimer.AddMillisecondsForNowTimeSpan(1000);
        if (keys.TryAdd(key, key))
            waitExpireds.Enqueue(key);
        else if (keys.TryAdd(key, key))
            waitExpireds.Enqueue(key);
        else if (keys.TryAdd(key, key))
            waitExpireds.Enqueue(key);

        return Ok(waitExpireds.Count);
    }

    private static void RemoveExpired()
    {
        while (waitExpireds.TryPeek(out var item) &&
            item <= GlobalTimer.NowTimeSpan())
        {
            var got = false;
            lock (lockObj)
            {
                if (waitExpireds.TryPeek(out item) &&
                    item <= GlobalTimer.NowTimeSpan() &&
                    waitExpireds.TryDequeue(out item))
                    got = true;
            }
            if (got)
            {
                keys.Remove(item, out var rm);
            }
        }
    }
}