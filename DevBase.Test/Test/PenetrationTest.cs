using System.Diagnostics;

namespace DevBase.Test.Test;

public class PenetrationTest
{
    protected PenetrationTest() {}
    
    public static Stopwatch Run(Action runAction, int count = 1_000_000)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        
        for (int i = 0; i < count; i++)
            runAction.Invoke();
        
        stopwatch.Stop();

        return stopwatch;
    }
    
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
