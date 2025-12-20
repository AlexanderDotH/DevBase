using System.Reflection;
using System.Text;
using DevBase.Requests.Data.Header;
using DevBase.Requests.Data.Header.UserAgent;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;
using Dumpify;

namespace DevBase.Test.DevBaseRequests.Preparation.Header;

public class RequestHeaderBuilderTest
{
    [Test]
    public void WithUserAgentTest()
    {
        Assembly assembly = this.GetType().Assembly;
        AssemblyName assemblyName = assembly.GetName();

        string userAgent = $"{assemblyName.Name}/{assemblyName.Version?.ToString()}";
        
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithUserAgent(userAgent)
            .Build();

        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Is.EquivalentTo(userAgent));

        buildedUserAgent.DumpConsole();
    }
    
    [Test]
    public void WithUserAgentBuilderTest()
    {
        Assembly assembly = this.GetType().Assembly;
        AssemblyName assemblyName = assembly.GetName();

        string userAgent = $"{assemblyName.Name}/{assemblyName.Version?.ToString()}";

        UserAgentHeaderBuilder userAgentBuilder = new UserAgentHeaderBuilder()
            .WithOverwrite(userAgent)
            .Build();
        
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithUserAgent(userAgentBuilder)
            .Build();
        
        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Is.EquivalentTo(userAgent));

        buildedUserAgent.DumpConsole();
    }
    
    [Test]
    public void WithBogusUserAgentTest()
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithBogusUserAgent()
            .Build();
        
        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Is.Not.Null);

        buildedUserAgent.DumpConsole();
    }

    [Test]
    public void WithBogusFirefoxUserAgentTest()
    {
        this.AssertUserAgentGenerator<BogusFirefoxUserAgentGenerator>("Firefox").DumpConsole();
    }
    
    [Test]
    public void WithBogusChromeUserAgentTest()
    {
        this.AssertUserAgentGenerator<BogusChromeUserAgentGenerator>("Chrome").DumpConsole();
    }
    
    [Test]
    public void WithBogusEdgeAgentTest()
    {
        this.AssertUserAgentGenerator<BogusEdgeUserAgentGenerator>("Edg").DumpConsole();
    }

    [Test]
    public void WithBogusOperaAgentTest()
    {
        this.AssertUserAgentGenerator<BogusOperaUserAgentGenerator>("OPR").DumpConsole();
    }
    
    private string AssertUserAgentGenerator<T>(string assertValue) 
        where T : IBogusUserAgentGenerator
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithBogusUserAgent<T>()
            .Build();
        
        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Does.Contain(assertValue));

        return buildedUserAgent;
    }

    [Test]
    public void WithBogusUserAgentT1T2()
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithBogusUserAgent<BogusFirefoxUserAgentGenerator, BogusChromeUserAgentGenerator>()
            .Build();
        
        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Is.Not.Null);

        buildedUserAgent.DumpConsole();
    }
    
    [Test]
    public void WithBogusUserAgentT1T2T3()
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithBogusUserAgent<BogusFirefoxUserAgentGenerator, BogusChromeUserAgentGenerator, BogusOperaUserAgentGenerator>()
            .Build();
        
        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Is.Not.Null);

        buildedUserAgent.DumpConsole();
    }
    
    [Test]
    public void WithBogusUserAgentT1T2T3T3()
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithBogusUserAgent<
                BogusFirefoxUserAgentGenerator, 
                BogusChromeUserAgentGenerator, 
                BogusOperaUserAgentGenerator, 
                BogusEdgeUserAgentGenerator>()
            .Build();
        
        string buildedUserAgent = builder["User-Agent"];
        Assert.That(buildedUserAgent, Is.Not.Null);

        buildedUserAgent.DumpConsole();
    }

    [Test]
    public void WithAcceptResolveTest()
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithAccept("json")
            .Build();

        string value = builder["Accept"];
        Assert.That(value, Is.EquivalentTo("application/json"));

        value.DumpConsole();
    }
    
    [Test]
    public void WithAcceptTest()
    {
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .WithAccept("custom-accept")
            .Build();

        string value = builder["Accept"];
        Assert.That(value, Is.EquivalentTo("custom-accept"));

        value.DumpConsole();
    }

    [Test]
    public void UseBasicAuthenticationTest()
    {
        string username = "admin";
        string password = "passw0rd";
        
        RequestHeaderBuilder builder = new RequestHeaderBuilder()
            .UseBasicAuthentication(username, password)
            .Build();

        string header = builder["Authorization"];

        int spaceIndex = header.IndexOf(' ');
        
        string authPrefix = header[..spaceIndex];
        string authContent = header[spaceIndex..];
        
        Assert.That(authPrefix, Is.EquivalentTo("Basic"));

        byte[] buffer = Convert.FromBase64String(authContent);

        byte[] eUsername = buffer.Take(..username.Length).ToArray();
        byte[] ePassword = buffer.TakeLast(password.Length).ToArray();
        
        string dUsername = Encoding.UTF8.GetString(eUsername);
        string dPassword= Encoding.UTF8.GetString(ePassword);
        
        Assert.That(dUsername, Is.EquivalentTo(username));
        Assert.That(dPassword, Is.EquivalentTo(password));

        dUsername.DumpConsole();
        dPassword.DumpConsole();
    }
}
