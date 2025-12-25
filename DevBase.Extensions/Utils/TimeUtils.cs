namespace DevBase.Extensions.Utils;

/// <summary>
/// Internal utility class for calculating time units from a stopwatch.
/// </summary>
internal class TimeUtils
{
    /// <summary>
    /// Gets the hours component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Hour/Hours).</returns>
    public static (int Hours, string Unit) GetHours(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Hours;
        string unit = "Hour";
        return (value, value > 1 ? unit + 's' : unit);
    }
   
    /// <summary>
    /// Gets the minutes component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Minute/Minutes).</returns>
    public static (int Minutes, string Unit) GetMinutes(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Minutes;
        string unit = "Minute";
        return (value, value == 1 ? unit : unit + 's');
    }

    /// <summary>
    /// Gets the seconds component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Second/Seconds).</returns>
    public static (int Seconds, string Unit) GetSeconds(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Seconds;
        string unit = "Second";
        return (value, value == 1 ? unit : unit + 's');
    }

    /// <summary>
    /// Gets the milliseconds component from the stopwatch elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Millisecond/Milliseconds).</returns>
    public static (int Milliseconds, string Unit) GetMilliseconds(System.Diagnostics.Stopwatch stopwatch)
    {
        int value = stopwatch.Elapsed.Milliseconds;
        string unit = "Millisecond";
        return (value, value == 1 ? unit : unit + 's');
    }

    /// <summary>
    /// Calculates the microseconds component from the stopwatch elapsed ticks.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Microsecond/Microseconds).</returns>
    public static (long Microseconds, string Unit) GetMicroseconds(System.Diagnostics.Stopwatch stopwatch)
    {
        long value = (stopwatch.ElapsedTicks / 10) % 1000;
        string unit = "Microsecond";
        return (value, value == 1 ? unit : unit + 's');
    }

    /// <summary>
    /// Calculates the nanoseconds component from the stopwatch elapsed ticks.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A tuple containing the value and the unit string (Nanosecond/Nanoseconds).</returns>
    public static (long Nanoseconds, string Unit) GetNanoseconds(System.Diagnostics.Stopwatch stopwatch)
    {
        long value = (stopwatch.ElapsedTicks % 10) * 1000;
        string unit = "Nanosecond";
        return (value, value == 1 ? unit : unit + 's');
    }
}