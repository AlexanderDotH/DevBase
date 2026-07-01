using System.Net;
using DevBase.IntegrationTests.Infrastructure;
using DevBase.Net.Core;
using DevBase.Net.Parsing;

namespace DevBase.IntegrationTests.Net;

[Category("Integration")]
[Category("Library")]
public sealed class NetLocalHttpIntegrationTests
{
    [Test]
    public async Task RequestCanSendHeadersAndQueryToLocalServerAndParseJsonResponse()
    {
        await using LocalHttpTestServer server = new(_ => LocalHttpResponse.Json(
            "{\"ok\":true,\"count\":2,\"items\":[{\"name\":\"alpha\"},{\"name\":\"beta\"}]}"));

        Response response = await new Request(new Uri(server.Url, "search"))
            .AsHttp1()
            .WithParameter("term", "devbase")
            .WithHeader("X-Integration-Test", "true")
            .WithTimeout(TimeSpan.FromSeconds(10))
            .SendAsync();
        CapturedHttpRequest request = await server.Request;
        List<JsonItem> items = await response.ParseJsonPathListAsync<JsonItem>("$.items[*]");
        MultiSelectorResult selected = await response.ParseMultipleJsonPathsAsync(
            CancellationToken.None,
            ("ok", "$.ok"),
            ("count", "$.count"));

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(request.Method, Is.EqualTo("GET"));
            Assert.That(request.Path, Is.EqualTo("/search"));
            Assert.That(request.Query, Is.EqualTo("term=devbase"));
            Assert.That(request.Headers["X-Integration-Test"], Is.EqualTo("true"));
            Assert.That(items.Select(item => item.name), Is.EqualTo(new[] { "alpha", "beta" }));
            Assert.That(selected.GetBool("ok"), Is.True);
            Assert.That(selected.GetInt("count"), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task RequestCanPostJsonBodyToLocalServer()
    {
        await using LocalHttpTestServer server = new(request => LocalHttpResponse.Json(
            $"{{\"method\":\"{request.Method}\",\"bodyLength\":{request.Body.Length}}}"));

        Response response = await new Request(new Uri(server.Url, "submit"))
            .AsHttp1()
            .AsPost()
            .WithJsonBody(new { name = "integration", amount = 3 })
            .WithTimeout(TimeSpan.FromSeconds(10))
            .SendAsync();
        CapturedHttpRequest request = await server.Request;
        string method = await response.ParseJsonPathAsync<string>("$.method");
        int bodyLength = await response.ParseJsonPathAsync<int>("$.bodyLength");

        Assert.Multiple(() =>
        {
            Assert.That(request.Method, Is.EqualTo("POST"));
            Assert.That(request.Path, Is.EqualTo("/submit"));
            Assert.That(request.Body, Does.Contain("\"name\":\"integration\""));
            Assert.That(method, Is.EqualTo("POST"));
            Assert.That(bodyLength, Is.EqualTo(request.Body.Length));
        });
    }

    private sealed class JsonItem
    {
        public string name { get; set; } = string.Empty;
    }
}
