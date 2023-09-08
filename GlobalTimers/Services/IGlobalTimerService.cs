namespace GlobalTimers.Services;

public interface IGlobalTimerService
{
    long AddMillisecondsForNowTicks(int addMilliseconds);
    TimeSpan AddMillisecondsForNowTimeSpan(int addMilliseconds);
    long AddTimeSpanForNowTicks(TimeSpan addTime);
    TimeSpan AddTimeSpanForNowTimeSpan(TimeSpan addTime);
    long NowTicks();
    TimeSpan NowTimeSpan();
}