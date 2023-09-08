using System.Diagnostics;

namespace GlobalTimers.Services;

public class GlobalTimerService : IGlobalTimerService
{
    public GlobalTimerService()
    {
    }

    public TimeSpan NowTimeSpan()
    {
        return new TimeSpan(NowTicks());
    }
    public TimeSpan AddTimeSpanForNowTimeSpan(TimeSpan addTime)
    {
        return NowTimeSpan().Add(addTime);
    }
    public TimeSpan AddMillisecondsForNowTimeSpan(int addMilliseconds)
    {
        return NowTimeSpan().Add(new TimeSpan(0, 0, 0, 0, addMilliseconds));
    }

    public long NowTicks()
    {
        return Stopwatch.GetTimestamp();
    }
    public long AddTimeSpanForNowTicks(TimeSpan addTime)
    {
        return AddTimeSpanForNowTimeSpan(addTime).Ticks;
    }
    public long AddMillisecondsForNowTicks(int addMilliseconds)
    {
        return AddMillisecondsForNowTimeSpan(addMilliseconds).Ticks;
    }
}