using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;
using DevBase.Requests.Utils;

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
        
        Console.WriteLine(sw.GetTimeTable().ToString());
        
        Console.WriteLine(generated.ToString());
    }
}