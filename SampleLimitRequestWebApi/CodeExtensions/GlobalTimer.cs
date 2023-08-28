using System.Diagnostics;

namespace SampleLimitRequestWebApi.CodeExtensions;

public static class GlobalTimer
{
    public static TimeSpan NowTimeSpan()
    {
        return new TimeSpan(NowTicks());
    }
    public static TimeSpan AddTimeSpanForNowTimeSpan(TimeSpan addTime)
    {
        return NowTimeSpan().Add(addTime);
    }
    public static TimeSpan AddMillisecondsForNowTimeSpan(int addMilliseconds)
    {
        return NowTimeSpan().Add(new TimeSpan(0, 0, 0, 0, addMilliseconds));
    }

    public static long NowTicks()
    {
        return Stopwatch.GetTimestamp();
    }
    public static long AddTimeSpanForNowTicks(TimeSpan addTime)
    {
        return AddTimeSpanForNowTimeSpan(addTime).Ticks;
    }
    public static long AddMillisecondsForNowTicks(int addMilliseconds)
    {
        return AddMillisecondsForNowTimeSpan(addMilliseconds).Ticks;
    }
}
