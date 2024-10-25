using DevBase.Requests.Data;
using DevBase.Requests.Data.Parameters;
using Dumpify;

namespace DevBase.Test.DevBaseRequests;

public class RequestBuilderTest
{
    [Test]
    public void BuildRequest()
    {
        ParameterBuilder parameterBuilder = new ParameterBuilder()
            .AddParameter("q", "DevBase GitHub")
            .AddParameter("cr", "countryDE");
        
        RequestBuilder requestBuilder = new RequestBuilder()
            .WithUrl("https://google.de/search")
            .WithParameters(parameterBuilder)
            .Build();

        string builtRequestUri = "https://google.de/search?q=DevBase GitHub&cr=countryDE";
        
        Assert.That(requestBuilder.Uri.ToString(), Is.EqualTo(builtRequestUri));
    }
}