using System.Text;
using System.Text.Json;
using DevBase.Requests.Configuration;
using DevBase.Requests.Constants;
using DevBase.Requests.Data.Body;
using DevBase.Requests.Data.Header;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Requests.Data.Parameters;
using DevBase.Requests.Interfaces;
using DevBase.Requests.Proxy;

namespace DevBase.Requests.Core;

public sealed partial class Request
{

    public Request WithUrl(string url)
    {
        this._requestBuilder.WithUrl(url);
        return this;
    }

    public Request WithUrl(Uri uri)
    {
        this._requestBuilder.WithUrl(uri);
        return this;
    }

    public Request WithParameters(ParameterBuilder parameterBuilder)
    {
        this._requestBuilder.WithParameters(parameterBuilder);
        return this;
    }

    public Request WithParameter(string key, string value)
    {
        ParameterBuilder builder = new ParameterBuilder();
        builder.AddParameter(key, value);
        return this.WithParameters(builder);
    }

    public Request WithParameters(params (string key, string value)[] parameters)
    {
        ParameterBuilder builder = new ParameterBuilder();
        builder.AddParameters(parameters);
        return this.WithParameters(builder);
    }

    public Request WithMethod(HttpMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        this._method = method;
        return this;
    }

    public Request AsGet() => this.WithMethod(HttpMethod.Get);
    public Request AsPost() => this.WithMethod(HttpMethod.Post);
    public Request AsPut() => this.WithMethod(HttpMethod.Put);
    public Request AsPatch() => this.WithMethod(HttpMethod.Patch);
    public Request AsDelete() => this.WithMethod(HttpMethod.Delete);
    public Request AsHead() => this.WithMethod(HttpMethod.Head);
    public Request AsOptions() => this.WithMethod(HttpMethod.Options);
    public Request AsTrace() => this.WithMethod(HttpMethod.Trace);

    public Request WithHeaders(RequestHeaderBuilder headerBuilder)
    {
        this._requestBuilder.WithHeaders(headerBuilder);
        return this;
    }

    public Request WithHeader(string name, string value)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.SetHeader(name, value);
        return this;
    }

    public Request WithHeader(ReadOnlyMemory<char> name, ReadOnlyMemory<char> value)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.SetHeader(name.ToString(), value.ToString());
        return this;
    }

    public Request WithHeader(ReadOnlyMemory<char> name, string value)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.SetHeader(name.ToString(), value);
        return this;
    }

    public Request WithAccept(ReadOnlyMemory<char> acceptType)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.WithAccept(acceptType.ToString());
        return this;
    }

    public Request WithAccept(params string[] acceptTypes)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.WithAccept(acceptTypes);
        return this;
    }

    public Request WithAcceptJson() => this.WithAccept(MimeConstants.Json.ToString());
    public Request WithAcceptXml() => this.WithAccept(MimeConstants.Xml.ToString());
    public Request WithAcceptHtml() => this.WithAccept(MimeConstants.Html.ToString());

    public Request WithUserAgent(string userAgent)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.WithUserAgent(userAgent);
        return this;
    }

    public Request WithBogusUserAgent()
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.WithBogusUserAgent();
        return this;
    }

    public Request WithBogusUserAgent<T>() where T : IBogusUserAgentGenerator
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.WithBogusUserAgent<T>();
        return this;
    }

    public Request WithBogusUserAgent<T1, T2>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.WithBogusUserAgent<T1, T2>();
        return this;
    }

    public Request WithReferer(string referer) => this.WithHeader(HeaderConstants.Referer.ToString(), referer);

    public Request WithCookie(string cookie) => this.WithHeader(HeaderConstants.Cookie.ToString(), cookie);

    private void EnsureHeaderBuilder()
    {
        if (this._requestBuilder.RequestHeaderBuilder == null)
            this._requestBuilder.WithHeaders(new RequestHeaderBuilder());
    }

    public Request UseBasicAuthentication(string username, string password)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.UseBasicAuthentication(username, password);
        return this;
    }

    public Request UseBearerAuthentication(string token)
    {
        this.EnsureHeaderBuilder();
        this._requestBuilder.RequestHeaderBuilder!.UseBearerAuthentication(token);
        return this;
    }

    public Request WithRawBody(RequestRawBodyBuilder bodyBuilder)
    {
        this._requestBuilder.WithRaw(bodyBuilder);
        return this;
    }

    public Request WithRawBody(string content, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        RequestRawBodyBuilder builder = new RequestRawBodyBuilder();
        builder.WithText(content, encoding);
        return this.WithRawBody(builder);
    }

    public Request WithJsonBody(string jsonContent, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        RequestRawBodyBuilder builder = new RequestRawBodyBuilder();
        builder.WithJson(jsonContent, encoding);
        return this.WithRawBody(builder);
    }

    public Request WithJsonBody<T>(T obj)
    {
        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return this.WithJsonBody(json, Encoding.UTF8);
    }

    public Request WithBufferBody(byte[] buffer)
    {
        RequestRawBodyBuilder builder = new RequestRawBodyBuilder();
        builder.WithBuffer(buffer);
        return this.WithRawBody(builder);
    }

    public Request WithBufferBody(Memory<byte> buffer) => this.WithBufferBody(buffer.ToArray());

    public Request WithEncodedForm(RequestEncodedKeyValueListBodyBuilder formBuilder)
    {
        this._requestBuilder.WithEncodedForm(formBuilder);
        return this;
    }

    public Request WithEncodedForm(params (string key, string value)[] formData)
    {
        RequestEncodedKeyValueListBodyBuilder builder = new RequestEncodedKeyValueListBodyBuilder();
        foreach ((string key, string value) in formData)
            builder.AddText(key, value);
        return this.WithEncodedForm(builder);
    }

    public Request WithForm(RequestKeyValueListBodyBuilder formBuilder)
    {
        this._requestBuilder.WithForm(formBuilder);
        return this;
    }

    public Request WithTimeout(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeout, TimeSpan.Zero);
        this._timeout = timeout;
        return this;
    }

    public Request WithCancellationToken(CancellationToken cancellationToken)
    {
        this._cancellationToken = cancellationToken;
        return this;
    }

    public Request WithProxy(TrackedProxyInfo? proxy)
    {
        this._proxy = proxy;
        return this;
    }

    public Request WithProxy(ProxyInfo proxy)
    {
        this._proxy = new TrackedProxyInfo(proxy);
        return this;
    }

    public Request WithRetryPolicy(RetryPolicy policy)
    {
        this._retryPolicy = policy ?? RetryPolicy.Default;
        return this;
    }

    public Request WithScrapingBypass(ScrapingBypassConfig config)
    {
        this._scrapingBypass = config;
        return this;
    }

    public Request WithJsonPathParsing(JsonPathConfig config)
    {
        this._jsonPathConfig = config;
        return this;
    }

    public Request WithHostCheck(HostCheckConfig config)
    {
        this._hostCheckConfig = config;
        return this;
    }

    public Request WithLogging(LoggingConfig config)
    {
        this._loggingConfig = config;
        return this;
    }

    public Request WithCertificateValidation(bool validate)
    {
        this._validateCertificates = validate;
        return this;
    }

    public Request WithFollowRedirects(bool follow, int maxRedirects = 50)
    {
        this._followRedirects = follow;
        this._maxRedirects = maxRedirects;
        return this;
    }

    public Request WithRequestInterceptor(IRequestInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);
        this._requestInterceptors.Add(interceptor);
        return this;
    }

    public Request WithResponseInterceptor(IResponseInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);
        this._responseInterceptors.Add(interceptor);
        return this;
    }
}
