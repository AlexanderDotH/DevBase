using ConsoleTables;
using DevBase.Extensions.Exceptions;
using DevBase.Extensions.Utils;

namespace DevBase.Extensions.Stopwatch;

public static class StopwatchExtension
{
    public static void PrintTimeTable(this System.Diagnostics.Stopwatch stopwatch) =>
        Console.WriteLine(stopwatch.GetTimeTable());
    
    public static string GetTimeTable(this System.Diagnostics.Stopwatch stopwatch)
    {
        if (stopwatch.IsRunning)
            throw new StopwatchException("The stopwatch should be stopped");

        ConsoleTable table = new ConsoleTable("Data", "Unit");
        table.Configure(c =>
        {
            c.NumberAlignment = Alignment.Right;
        });

        (int Hours, string Unit) hours = TimeUtils.GetHours(stopwatch);
        (int Minutes, string Unit) minutes = TimeUtils.GetMinutes(stopwatch);
        (int Seconds, string Unit) seconds = TimeUtils.GetSeconds(stopwatch);
        (int Milliseconds, string Unit) milliseconds = TimeUtils.GetMilliseconds(stopwatch);
        (long Microseconds, string Unit) microseconds = TimeUtils.GetMicroseconds(stopwatch);
        (long Nanoseconds, string Unit) nanoseconds = TimeUtils.GetNanoseconds(stopwatch);

        if (hours.Hours > 0)
            table.AddRow(hours.Hours, hours.Unit);

        if (minutes.Minutes > 0)
            table.AddRow(minutes.Minutes, minutes.Unit);

        if (seconds.Seconds > 0)
            table.AddRow(seconds.Seconds, seconds.Unit);

        if (milliseconds.Milliseconds > 0)
            table.AddRow(milliseconds.Milliseconds, milliseconds.Unit);

        if (microseconds.Microseconds > 0)
            table.AddRow(microseconds.Microseconds, microseconds.Unit);

        if (nanoseconds.Nanoseconds > 0)
            table.AddRow(nanoseconds.Nanoseconds, nanoseconds.Unit);

        return table.ToMarkDownString();
    } 
}