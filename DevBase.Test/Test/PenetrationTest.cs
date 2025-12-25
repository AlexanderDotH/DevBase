using System.Diagnostics;

namespace DevBase.Test.Test;

/// <summary>
/// Helper class for performance testing (penetration testing).
/// </summary>
public class PenetrationTest
{
    protected PenetrationTest() {}
    
    /// <summary>
    /// Runs an action multiple times and measures the total execution time.
    /// </summary>
    /// <param name="runAction">The action to execute.</param>
    /// <param name="count">The number of times to execute the action.</param>
    /// <returns>A Stopwatch instance with the elapsed time.</returns>
    public static Stopwatch Run(Action runAction, int count = 1_000_000)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        
        for (int i = 0; i < count; i++)
            runAction.Invoke();
        
        stopwatch.Stop();

        return stopwatch;
    }
    
    /// <summary>
    /// Runs a function multiple times and returns the output of the last execution.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="runAction">The function to execute.</param>
    /// <param name="lastActionOutput">The output of the last execution.</param>
    /// <param name="count">The number of times to execute the function.</param>
    /// <returns>A Stopwatch instance with the elapsed time.</returns>
    public static Stopwatch RunWithLast<T>(Func<T> runAction, out T lastActionOutput, int count = 1_000_000)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        
        // For gods sake please don't specify count = 0 or some bs :)
        for (int i = 0; i < count - 1; i++)
            lastActionOutput = runAction.Invoke();

        lastActionOutput = runAction.Invoke();
        
        stopwatch.Stop();
        return stopwatch;
    }
}
