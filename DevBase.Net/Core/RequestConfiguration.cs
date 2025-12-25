using System.Text;
using System.Text.Json;
using DevBase.Net.Configuration;
using DevBase.Net.Constants;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Header;
using DevBase.Net.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Net.Data.Parameters;
using DevBase.Net.Interfaces;
using DevBase.Net.Proxy;
using DevBase.Net.Security.Token;

namespace DevBase.Net.Core;

public partial class Request
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

    public Request UseJwtAuthentication(AuthenticationToken token)
    {
        return UseBearerAuthentication(token.RawToken);
    }

    public Request UseJwtAuthentication(string rawToken)
    {
        AuthenticationToken? token = AuthenticationToken.FromString(rawToken);
        if (token == null)
            throw new ArgumentException("Invalid JWT token format", nameof(rawToken));
        return UseBearerAuthentication(token.RawToken);
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

    /// <summary>
    /// Sets a proxy using a proxy string in format: [protocol://][user:pass@]host:port
    /// Supported protocols: http, https, socks4, socks5, socks5h, ssh
    /// Examples:
    /// - socks5://user:pass@host:port
    /// - http://proxy.example.com:8080
    /// - socks5h://user:pass@host:port (remote DNS resolution)
    /// </summary>
    public Request WithProxy(string proxyString)
    {
        return WithProxy(ProxyInfo.Parse(proxyString));
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

    public Request WithHeaderValidation(bool validate)
    {
        this._validateHeaders = validate;
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
