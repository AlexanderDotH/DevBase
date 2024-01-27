using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusUserAgentGeneratorTests
{
    [Test]
    public void GenerateBogusChromeUserAgent()
    {
        GenerateAndTestUserAgent(new BogusChromeUserAgentGenerator());
    }
    
    [Test]
    public void GenerateBogusEdgeUserAgent()
    {
        GenerateAndTestUserAgent(new BogusEdgeUserAgentGenerator());
    }

    [Test]
    public void GenerateBogusFirefoxUserAgent()
    {
        GenerateAndTestUserAgent(new BogusFirefoxUserAgentGenerator());
    }

    [Test]
    public void GenerateBogusOperaUserAgent()
    {
        GenerateAndTestUserAgent(new BogusOperaUserAgentGenerator());
    }
    
    private void GenerateAndTestUserAgent(IBogusUserAgentGenerator generator, int count = 1_000_000)
    {
        Stopwatch sw = new Stopwatch();
        
        sw.Start();

        for (int i = 0; i < count; i++)
        {
            ReadOnlySpan<char> generated = generator.UserAgentPart;
        }
        
        sw.Stop();

        Console.WriteLine($"Generated user-agent of type {generator.GetType().Name}\n");
        Console.WriteLine(sw.GetTimeTable());
        Console.WriteLine($"The User-Agent: {generator.UserAgentPart}");
    }
}