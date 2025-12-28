using System.Net.Http.Headers;
using DevBase.Net.Core;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class ContentTypeHeaderTest
{
    [Test]
    public void ExplicitContentType_ShouldBeAppliedToContentHeaders()
    {
        Request request = new Request("https://example.com/api")
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody("{\"test\": \"value\"}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        Assert.That(httpMessage.Content, Is.Not.Null);
        Assert.That(httpMessage.Content!.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public void ExplicitContentType_ShouldNotBeInRequestHeaders()
    {
        Request request = new Request("https://example.com/api")
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody("{\"test\": \"value\"}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        bool hasContentTypeInRequestHeaders = httpMessage.Headers
            .Any(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
        Assert.That(hasContentTypeInRequestHeaders, Is.False, "Content-Type should not be in request headers");
    }

    [Test]
    public void CustomContentType_ShouldBePreserved()
    {
        const string customContentType = "application/vnd.api+json";
        
        Request request = new Request("https://example.com/api")
            .WithHeader("Content-Type", customContentType)
            .WithJsonBody("{\"data\": {}}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        Assert.That(httpMessage.Content, Is.Not.Null);
        Assert.That(httpMessage.Content!.Headers.ContentType?.MediaType, Is.EqualTo(customContentType));
    }

    [Test]
    public void NoExplicitContentType_ShouldDefaultToJsonForJsonBody()
    {
        Request request = new Request("https://example.com/api")
            .WithJsonBody("{\"test\": \"value\"}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        Assert.That(httpMessage.Content, Is.Not.Null);
        Assert.That(httpMessage.Content!.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public void ContentTypeWithCharset_ShouldBePreserved()
    {
        const string contentTypeWithCharset = "application/json; charset=utf-8";
        
        Request request = new Request("https://example.com/api")
            .WithHeader("Content-Type", contentTypeWithCharset)
            .WithJsonBody("{\"test\": \"value\"}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        Assert.That(httpMessage.Content, Is.Not.Null);
        string? fullContentType = httpMessage.Content!.Headers.ContentType?.ToString();
        Assert.That(fullContentType, Is.EqualTo(contentTypeWithCharset));
    }

    [Test]
    public void OtherHeaders_ShouldStillBeInRequestHeaders()
    {
        Request request = new Request("https://example.com/api")
            .WithHeader("Content-Type", "application/json")
            .WithHeader("X-Custom-Header", "custom-value")
            .WithHeader("Authorization", "Bearer token123")
            .WithJsonBody("{\"test\": \"value\"}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        Assert.That(httpMessage.Headers.Contains("X-Custom-Header"), Is.True);
        Assert.That(httpMessage.Headers.Contains("Authorization"), Is.True);
        Assert.That(httpMessage.Headers.GetValues("X-Custom-Header").First(), Is.EqualTo("custom-value"));
    }

    [Test]
    public void ContentTypeCaseInsensitive_ShouldBeHandledCorrectly()
    {
        Request request = new Request("https://example.com/api")
            .WithHeader("content-type", "application/json")
            .WithJsonBody("{\"test\": \"value\"}");

        using HttpRequestMessage httpMessage = request.ToHttpRequestMessage();

        bool hasContentTypeInRequestHeaders = httpMessage.Headers
            .Any(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
        Assert.That(hasContentTypeInRequestHeaders, Is.False, "Content-Type (any case) should not be in request headers");
        
        Assert.That(httpMessage.Content, Is.Not.Null);
        Assert.That(httpMessage.Content!.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }
}
