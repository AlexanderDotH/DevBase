using DevBase.Requests.Data.Header.UserAgent;
using DevBase.Requests.Exceptions;

namespace DevBase.Test.DevBaseRequests.Builder;

public class UserAgentBuilderTest
{
    [Test]
    public void BuildBogusUserAgentTest()
    {
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder().BuildBogus();

        Assert.NotNull(builder.UserAgent.ToString());
        
        Console.WriteLine($"Generated random user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomUserAgentTest()
    {
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder()
            .AddProductName("Microsoft Excel ;)")
            .AddProductVersion("1.0")
            .Build();

        Assert.NotNull(builder.UserAgent.ToString());
        
        Console.WriteLine($"Built user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomBogusUserAgentTest()
    {
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder()
            .AddProductName("Microsoft Excel ;)")
            .AddProductVersion("1.0")
            .Build()
            .BuildBogus();

        Assert.NotNull(builder.UserAgent.ToString());
        
        Console.WriteLine($"Built user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomUserAgentWithTest()
    {
        string userAgent = "Mozilla/5.0 (X11; Linux i686; rv:13.0) Gecko/13.0 Firefox/13.0";
        
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder()
            .With(userAgent)
            .Build();

        Assert.That(builder.UserAgent.ToString(), Is.EqualTo(userAgent));
        
        Console.WriteLine($"Built user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomBogusCustomUserAgentTest()
    {
        Assert.Throws<HttpHeaderException>(() =>
        {
            new UserAgentHeaderBuilder()
                .AddProductName("Microsoft Excel ;)")
                .AddProductVersion("1.0")
                .Build() // Build 1
                .BuildBogus() // Build 2
                .Build(); // Build 3
        });
        
        Console.WriteLine($"It should throw an \"HttpHeaderException\" and it worked!");
    }
}