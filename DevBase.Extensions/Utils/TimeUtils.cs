namespace DevBase.Extensions.Utils;

internal class TimeUtils
{
    public static (int Hours, string Unit) GetHours(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Hours;
        string unit = "Hour";
        return (value, value > 1 ? unit + 's' : unit);
    }
   
    public static (int Minutes, string Unit) GetMinutes(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Minutes;
        string unit = "Minute";
        return (value, value == 1 ? unit : unit + 's');
    }

    public static (int Seconds, string Unit) GetSeconds(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Seconds;
        string unit = "Second";
        return (value, value == 1 ? unit : unit + 's');
    }

    public static (int Milliseconds, string Unit) GetMilliseconds(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Milliseconds;
        string unit = "Millisecond";
        return (value, value == 1 ? unit : unit + 's');
    }

    public static (long Microseconds, string Unit) GetMicroseconds(System.Diagnostics.Stopwatch stopwatch)
    {
        long value = (stopwatch.ElapsedTicks / 10) % 1000;
        string unit = "Microsecond";
        return (value, value == 1 ? unit : unit + 's');
    }

    public static (long Nanoseconds, string Unit) GetNanoseconds(System.Diagnostics.Stopwatch stopwatch)
    {
        long value = (stopwatch.ElapsedTicks % 10) * 1000;
        string unit = "Nanosecond";
        return (value, value == 1 ? unit : unit + 's');
    }
}