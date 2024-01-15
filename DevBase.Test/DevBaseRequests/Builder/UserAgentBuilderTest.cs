using System.Diagnostics;
using DevBase.Requests.Preparation.Header.UserAgent;

namespace DevBase.Test.DevBaseRequests.Builder;

public class UserAgentBuilderTest
{
    [Test]
    public void BuildUserAgentTest()
    {
        int count = 1_000_000;
        
        Stopwatch sw = new Stopwatch();
        
        sw.Start();
        
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder();

        for (int i = 0; i < count; i++)
            builder = new UserAgentHeaderBuilder().Build();

        ReadOnlySpan<char> agent = builder.UserAgent;
        
        sw.Stop();
        
        Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms or {sw.ElapsedTicks}ts to calculate the user-agent {count}times");
        Console.WriteLine($"User-Agent: {agent}");
    }
}