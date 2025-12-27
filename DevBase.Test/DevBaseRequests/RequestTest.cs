using System.Net;
using System.Text;
using DevBase.Net;
using DevBase.Net.Configuration;
using DevBase.Net.Core;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Header;
using DevBase.Net.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Net.Data.Parameters;
using DevBase.Net.Proxy;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class RequestTest
{
    #region Constructor Tests

    [Test]
    public void Constructor_Default_CreatesEmptyRequest()
    {
        var request = new Request();
        
        Assert.That(request.Uri.IsEmpty, Is.True);
        Assert.That(request.Body.IsEmpty, Is.True);
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
    }

    [Test]
    public void Constructor_WithStringUrl_SetsUri()
    {
        var request = new Request("https://example.com/api");
        
        Assert.That(request.Uri.ToString(), Is.EqualTo("https://example.com/api"));
    }

    [Test]
    public void Constructor_WithUri_SetsUri()
    {
        var uri = new Uri("https://example.com/api");
        var request = new Request(uri);
        
        Assert.That(request.GetUri()?.ToString(), Is.EqualTo("https://example.com/api"));
    }

    [Test]
    public void Constructor_WithUrlAndMethod_SetsBoth()
    {
        var request = new Request("https://example.com/api", HttpMethod.Post);
        
        Assert.That(request.Uri.ToString(), Is.EqualTo("https://example.com/api"));
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
    }

    #endregion

    #region URL Builder Tests

    [Test]
    public void WithUrl_String_SetsUri()
    {
        var request = new Request()
            .WithUrl("https://api.example.com/v1");
        
        Assert.That(request.Uri.ToString(), Is.EqualTo("https://api.example.com/v1"));
    }

    [Test]
    public void WithUrl_Uri_SetsUri()
    {
        var request = new Request()
            .WithUrl(new Uri("https://api.example.com/v1"));
        
        Assert.That(request.GetUri()?.Host, Is.EqualTo("api.example.com"));
    }

    [Test]
    public void WithParameter_SingleParameter_AddsParameter()
    {
        var request = new Request("https://example.com/api")
            .WithParameter("key", "value")
            .Build();
        
        Assert.That(request.Uri.ToString(), Does.Contain("key=value"));
    }

    [Test]
    public void WithParameters_MultipleParameters_AddsAllParameters()
    {
        var request = new Request("https://example.com/api")
            .WithParameters(("page", "1"), ("limit", "10"))
            .Build();
        
        var uriString = request.Uri.ToString();
        Assert.That(uriString, Does.Contain("page=1"));
        Assert.That(uriString, Does.Contain("limit=10"));
    }

    [Test]
    public void WithParameters_ParameterBuilder_UsesBuilder()
    {
        var paramBuilder = new ParameterBuilder();
        paramBuilder.AddParameter("search", "test");
        paramBuilder.AddParameter("sort", "desc");
        
        var request = new Request("https://example.com/api")
            .WithParameters(paramBuilder)
            .Build();
        
        var uriString = request.Uri.ToString();
        Assert.That(uriString, Does.Contain("search=test"));
        Assert.That(uriString, Does.Contain("sort=desc"));
    }

    #endregion

    #region Method Builder Tests

    [Test]
    public void AsGet_SetsMethodToGet()
    {
        var request = new Request("https://example.com").AsGet();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
    }

    [Test]
    public void AsPost_SetsMethodToPost()
    {
        var request = new Request("https://example.com").AsPost();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
    }

    [Test]
    public void AsPut_SetsMethodToPut()
    {
        var request = new Request("https://example.com").AsPut();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Put));
    }

    [Test]
    public void AsPatch_SetsMethodToPatch()
    {
        var request = new Request("https://example.com").AsPatch();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Patch));
    }

    [Test]
    public void AsDelete_SetsMethodToDelete()
    {
        var request = new Request("https://example.com").AsDelete();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Delete));
    }

    [Test]
    public void AsHead_SetsMethodToHead()
    {
        var request = new Request("https://example.com").AsHead();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Head));
    }

    [Test]
    public void AsOptions_SetsMethodToOptions()
    {
        var request = new Request("https://example.com").AsOptions();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Options));
    }

    [Test]
    public void WithMethod_CustomMethod_SetsMethod()
    {
        var request = new Request("https://example.com")
            .WithMethod(HttpMethod.Trace);
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Trace));
    }

    #endregion

    #region Headers Builder Tests

    [Test]
    public void WithHeader_SingleHeader_AddsHeader()
    {
        var request = new Request("https://example.com")
            .WithHeader("X-Custom-Header", "CustomValue")
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("X-Custom-Header"), Is.True);
    }

    [Test]
    public void WithHeaders_RequestHeaderBuilder_UsesBuilder()
    {
        var headerBuilder = new RequestHeaderBuilder();
        headerBuilder["X-Test"] = "TestValue";
        
        var request = new Request("https://example.com")
            .WithHeaders(headerBuilder)
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("X-Test"), Is.True);
    }

    [Test]
    public void WithUserAgent_SetsUserAgentHeader()
    {
        var request = new Request("https://example.com")
            .WithUserAgent("TestAgent/1.0")
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.UserAgent.ToString(), Does.Contain("TestAgent"));
    }

    [Test]
    public void WithBogusUserAgent_GeneratesUserAgent()
    {
        var request = new Request("https://example.com")
            .WithBogusUserAgent()
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("User-Agent"), Is.True);
    }

    [Test]
    public void WithBogusUserAgent_Chrome_GeneratesChromeUserAgent()
    {
        var request = new Request("https://example.com")
            .WithBogusUserAgent<BogusChromeUserAgentGenerator>()
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("User-Agent"), Is.True);
    }

    [Test]
    public void WithAcceptJson_SetsAcceptHeader()
    {
        var request = new Request("https://example.com")
            .WithAcceptJson()
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Accept.ToString(), Does.Contain("application/json"));
    }

    [Test]
    public void WithReferer_SetsRefererHeader()
    {
        var request = new Request("https://example.com")
            .WithReferer("https://google.com")
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Referrer?.ToString(), Does.StartWith("https://google.com"));
    }

    [Test]
    public void WithCookie_SetsCookieHeader()
    {
        var request = new Request("https://example.com")
            .WithCookie("session=abc123")
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("Cookie"), Is.True);
    }

    #endregion

    #region Authentication Builder Tests

    [Test]
    public void UseBearerAuthentication_SetsAuthorizationHeader()
    {
        var request = new Request("https://example.com")
            .UseBearerAuthentication("test-token-123")
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("Authorization"), Is.True);
    }

    [Test]
    public void UseBasicAuthentication_SetsAuthorizationHeader()
    {
        var request = new Request("https://example.com")
            .UseBasicAuthentication("user", "pass")
            .Build();
        
        var httpMessage = request.ToHttpRequestMessage();
        Assert.That(httpMessage.Headers.Contains("Authorization"), Is.True);
    }

    #endregion

    #region Body Builder Tests

    [Test]
    public void WithRawBody_String_SetsBody()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .WithRawBody("Hello World")
            .Build();
        
        Assert.That(request.Body.IsEmpty, Is.False);
        Assert.That(Encoding.UTF8.GetString(request.Body), Is.EqualTo("Hello World"));
    }

    [Test]
    public void WithJsonBody_String_SetsJsonBody()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .WithJsonBody("{\"key\":\"value\"}", Encoding.UTF8)
            .Build();
        
        Assert.That(request.Body.IsEmpty, Is.False);
        var bodyString = Encoding.UTF8.GetString(request.Body);
        Assert.That(bodyString, Does.Contain("key"));
    }

    [Test]
    public void WithJsonBody_Object_SerializesAndSetsBody()
    {
        var testObject = new { Name = "Test", Value = 42 };
        
        var request = new Request("https://example.com")
            .AsPost()
            .WithJsonBody(testObject)
            .Build();
        
        Assert.That(request.Body.IsEmpty, Is.False);
        var bodyString = Encoding.UTF8.GetString(request.Body);
        Assert.That(bodyString, Does.Contain("name"));
        Assert.That(bodyString, Does.Contain("Test"));
    }

    [Test]
    public void WithBufferBody_ByteArray_SetsBody()
    {
        var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        
        var request = new Request("https://example.com")
            .AsPost()
            .WithBufferBody(buffer)
            .Build();
        
        Assert.That(request.Body.ToArray(), Is.EqualTo(buffer));
    }

    [Test]
    public void WithBufferBody_Memory_SetsBody()
    {
        var buffer = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        Memory<byte> memory = buffer;
        
        var request = new Request("https://example.com")
            .AsPost()
            .WithBufferBody(memory)
            .Build();
        
        Assert.That(request.Body.ToArray(), Is.EqualTo(buffer));
    }

    [Test]
    public void WithEncodedForm_Tuples_SetsFormBody()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .WithEncodedForm(("username", "test"), ("password", "secret"))
            .Build();
        
        Assert.That(request.Body.IsEmpty, Is.False);
    }

    [Test]
    public void WithEncodedForm_Builder_SetsFormBody()
    {
        var formBuilder = new RequestEncodedKeyValueListBodyBuilder();
        formBuilder.AddText("field1", "value1");
        formBuilder.AddText("field2", "value2");
        
        var request = new Request("https://example.com")
            .AsPost()
            .WithEncodedForm(formBuilder)
            .Build();
        
        Assert.That(request.Body.IsEmpty, Is.False);
    }

    [Test]
    public void WithRawBody_RequestRawBodyBuilder_SetsBody()
    {
        var bodyBuilder = new RequestRawBodyBuilder();
        bodyBuilder.WithText("Test Content", Encoding.UTF8);
        
        var request = new Request("https://example.com")
            .AsPost()
            .WithRawBody(bodyBuilder)
            .Build();
        
        Assert.That(request.Body.IsEmpty, Is.False);
    }

    #endregion

    #region Configuration Builder Tests

    [Test]
    public void WithTimeout_SetsTimeout()
    {
        var timeout = TimeSpan.FromSeconds(60);
        
        var request = new Request("https://example.com")
            .WithTimeout(timeout);
        
        Assert.That(request.Timeout, Is.EqualTo(timeout));
    }

    [Test]
    public void WithTimeout_ZeroOrNegative_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Request("https://example.com").WithTimeout(TimeSpan.Zero));
        
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Request("https://example.com").WithTimeout(TimeSpan.FromSeconds(-1)));
    }

    [Test]
    public void WithCancellationToken_SetsCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        
        var request = new Request("https://example.com")
            .WithCancellationToken(cts.Token);
        
        Assert.That(request.CancellationToken, Is.EqualTo(cts.Token));
    }

    [Test]
    public void WithProxy_ProxyInfo_SetsProxy()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        
        var request = new Request("https://example.com")
            .WithProxy(proxyInfo);
        
        Assert.That(request.Proxy, Is.Not.Null);
        Assert.That(request.Proxy?.Proxy.Host, Is.EqualTo("proxy.example.com"));
    }

    [Test]
    public void WithProxy_TrackedProxyInfo_SetsProxy()
    {
        var proxyInfo = ProxyInfo.Parse("http://proxy.example.com:8080");
        var trackedProxy = new TrackedProxyInfo(proxyInfo);
        
        var request = new Request("https://example.com")
            .WithProxy(trackedProxy);
        
        Assert.That(request.Proxy, Is.EqualTo(trackedProxy));
    }

    [Test]
    public void WithRetryPolicy_SetsRetryPolicy()
    {
        var policy = RetryPolicy.Aggressive;
        
        var request = new Request("https://example.com")
            .WithRetryPolicy(policy);
        
        Assert.That(request.RetryPolicy, Is.EqualTo(policy));
    }

    [Test]
    public void WithCertificateValidation_SetsCertificateValidation()
    {
        var request = new Request("https://example.com")
            .WithCertificateValidation(false);
        
        Assert.That(request.ValidateCertificates, Is.False);
    }

    [Test]
    public void WithFollowRedirects_SetsFollowRedirects()
    {
        var request = new Request("https://example.com")
            .WithFollowRedirects(false);
        
        Assert.That(request.FollowRedirects, Is.False);
    }

    [Test]
    public void WithFollowRedirects_WithMaxRedirects_SetsBoth()
    {
        var request = new Request("https://example.com")
            .WithFollowRedirects(true, 10);
        
        Assert.That(request.FollowRedirects, Is.True);
        Assert.That(request.MaxRedirects, Is.EqualTo(10));
    }

    #endregion

    #region Build Tests

    [Test]
    public void Build_ValidRequest_Succeeds()
    {
        var request = new Request("https://example.com")
            .AsGet()
            .Build();
        
        Assert.That(request.Uri.IsEmpty, Is.False);
    }

    [Test]
    public void Build_MultipleCalls_OnlyBuildsOnce()
    {
        var request = new Request("https://example.com")
            .WithParameter("key", "value");
        
        request.Build();
        var uri1 = request.Uri.ToString();
        
        request.Build();
        var uri2 = request.Uri.ToString();
        
        Assert.That(uri1, Is.EqualTo(uri2));
    }

    [Test]
    public void ToHttpRequestMessage_CreatesValidMessage()
    {
        var request = new Request("https://example.com/api")
            .AsPost()
            .WithHeader("X-Test", "Value")
            .WithJsonBody("{\"test\":true}", Encoding.UTF8)
            .Build();
        
        var message = request.ToHttpRequestMessage();
        
        Assert.That(message.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(message.RequestUri?.ToString(), Is.EqualTo("https://example.com/api"));
        Assert.That(message.Content, Is.Not.Null);
    }

    #endregion

    #region Disposal Tests

    [Test]
    public void Dispose_ClearsInterceptors()
    {
        var request = new Request("https://example.com");
        request.Dispose();
        
        Assert.That(request.RequestInterceptors.Count, Is.EqualTo(0));
        Assert.That(request.ResponseInterceptors.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task DisposeAsync_ClearsInterceptors()
    {
        var request = new Request("https://example.com");
        await request.DisposeAsync();
        
        Assert.That(request.RequestInterceptors.Count, Is.EqualTo(0));
        Assert.That(request.ResponseInterceptors.Count, Is.EqualTo(0));
    }

    #endregion

    #region HTTP Version Tests

    [Test]
    public void WithHttpVersion_SetsHttpVersion()
    {
        var request = new Request("https://example.com")
            .WithHttpVersion(HttpVersion.Version20);
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version20));
    }

    [Test]
    public void WithHttpVersion_WithPolicy_SetsBoth()
    {
        var request = new Request("https://example.com")
            .WithHttpVersion(HttpVersion.Version20, HttpVersionPolicy.RequestVersionExact);
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version20));
        Assert.That(request.HttpVersionPolicy, Is.EqualTo(HttpVersionPolicy.RequestVersionExact));
    }

    [Test]
    public void AsHttp1_SetsHttpVersion11()
    {
        var request = new Request("https://example.com")
            .AsHttp1();
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version11));
    }

    [Test]
    public void AsHttp2_SetsHttpVersion20()
    {
        var request = new Request("https://example.com")
            .AsHttp2();
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version20));
    }

    [Test]
    public void AsHttp3_SetsHttpVersion30()
    {
        var request = new Request("https://example.com")
            .AsHttp3();
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version30));
    }

    [Test]
    public void DefaultHttpVersion_IsHttp3()
    {
        var request = new Request("https://example.com");
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version30));
        Assert.That(request.HttpVersionPolicy, Is.EqualTo(HttpVersionPolicy.RequestVersionOrLower));
    }

    [Test]
    public void HttpVersion_CanBeSwitchedMultipleTimes()
    {
        var request = new Request("https://example.com")
            .AsHttp3()
            .AsHttp2()
            .AsHttp1();
        
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version11));
    }

    [Test]
    public void FluentApi_WithHttpVersion_ChainsCorrectly()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .AsHttp2()
            .WithHeader("X-Test", "Value")
            .Build();
        
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(request.HttpVersion, Is.EqualTo(HttpVersion.Version20));
    }

    #endregion

    #region Fluent API Tests

    [Test]
    public void FluentApi_MethodChaining_Works()
    {
        var request = new Request()
            .WithUrl("https://api.example.com/v1/users")
            .AsPost()
            .WithHeader("X-Api-Key", "secret")
            .WithBogusUserAgent()
            .UseBearerAuthentication("valid-test-token-12345")
            .WithJsonBody(new { name = "Test" })
            .WithTimeout(TimeSpan.FromSeconds(30))
            .WithCertificateValidation(true)
            .WithFollowRedirects(true, 5)
            .WithRetryPolicy(RetryPolicy.Default)
            .Build();
        
        Assert.That(request.Uri.ToString(), Does.Contain("api.example.com"));
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(request.Timeout, Is.EqualTo(TimeSpan.FromSeconds(30)));
        Assert.That(request.Body.IsEmpty, Is.False);
    }

    #endregion
}
