using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Net.Utils;

namespace DevBase.Test.DevBaseRequests.Utils;

public class BogusUtilsTests
{
    [Test]
    public void RandomNumber()
    {
        int count = 1_000_000;
        
        Stopwatch sw = new Stopwatch();
        
        sw.Start();

        ReadOnlySpan<char> generated = null;
        
        for (int i = 0; i < count; i++)
        {
            generated = BogusUtils.RandomNumber(0, 1000);
        }
        
        sw.Stop();
        
        sw.PrintTimeTable();

        Console.WriteLine(generated.ToString());
        
        Assert.That(generated.ToString(), Is.Not.Null);
    }
}
