using System.Diagnostics;

namespace DevBase.Test.Test;

public class PenetrationTest
{
    public static Stopwatch Run(Action runAction, int count = 1_000_000)
    {
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();
        
        for (int i = 0; i < count; i++)
            runAction.Invoke();
        
        stopwatch.Stop();

        return stopwatch;
    }
}