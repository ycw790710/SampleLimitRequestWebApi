using System.Diagnostics;

namespace CodeExtensions;

public class GlobalTimerService
{
    private TimeSpan _offset;

    public GlobalTimerService()
    {
        _offset = TimeSpan.Zero;
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