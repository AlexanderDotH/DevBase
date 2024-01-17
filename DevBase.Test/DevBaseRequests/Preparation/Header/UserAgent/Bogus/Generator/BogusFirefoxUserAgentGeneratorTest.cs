using System.Diagnostics;
using DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusFirefoxUserAgentGeneratorTest
{
    [Test]
    public void UserAgentPartTest()
    {
        int count = 1_000_000;
        
        Stopwatch sw = new Stopwatch();
        
        sw.Start();

        BogusFirefoxUserAgentGenerator generator = new BogusFirefoxUserAgentGenerator();
        
        for (int i = 0; i < count; i++)
        {
            ReadOnlySpan<char> generated = generator.UserAgentPart;
        }
        
        sw.Stop();
        
        Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms or {sw.ElapsedTicks}ts to generate {count} user-agents");
        
        Console.WriteLine(generator.UserAgentPart.ToString());
    }
}