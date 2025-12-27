using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;
using DevBase.Net.Constants;
using DevBase.Net.Exceptions;
using DevBase.Net.Spoofing;
using DevBase.Net.Metrics;
using DevBase.Net.Proxy.Enums;
using DevBase.Net.Utils;
using DevBase.Net.Validation;

namespace DevBase.Net.Core;

public partial class Request
{
    public override Request Build()
    {
        if (this._isBuilt)
            return this;
        
        List<KeyValuePair<string, string>>? userHeaders = null;
        string? userDefinedUserAgent = null;
        
        if (this._requestBuilder.RequestHeaderBuilder != null)
        {
            userHeaders = this._requestBuilder.RequestHeaderBuilder.GetEntries().ToList();
            userDefinedUserAgent = this._requestBuilder.RequestHeaderBuilder.GetPreBuildUserAgent();
        }
        
        ApplyScrapingBypassIfConfigured(userHeaders, userDefinedUserAgent);
        
        this._requestBuilder.Build();
        
        if (this._validateHeaders)
            ValidateHeaders();
        
        this._isBuilt = true;
        return this;
    }

    private void ValidateHeaders()
    {
        Data.Header.RequestHeaderBuilder? headers = this._requestBuilder.RequestHeaderBuilder;
        if (headers == null)
            return;

        foreach (KeyValuePair<string, string> header in headers.GetEntries())
        {
            string name = header.Key;
            string value = header.Value;
            ValidationResult result;

            if (name.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                if (value.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                    result = HeaderValidator.ValidateBasicAuth(value);
                else if (value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    result = HeaderValidator.ValidateBearerAuth(value);
                else
                    continue;
            }
            else if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                result = HeaderValidator.ValidateContentType(value);
            }
            else if (name.Equals("Accept", StringComparison.OrdinalIgnoreCase))
            {
                result = HeaderValidator.ValidateAccept(value);
            }
            else if (name.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
            {
                result = HeaderValidator.ValidateCookie(value);
            }
            else if (name.Equals("Accept-Encoding", StringComparison.OrdinalIgnoreCase))
            {
                result = HeaderValidator.ValidateAcceptEncoding(value);
            }
            else
            {
                continue;
            }

            if (!result.IsValid)
                throw new HeaderValidationException(name, result.ErrorMessage ?? "Validation failed");
        }
    }

    public override async Task<Response> SendAsync(CancellationToken cancellationToken = default)
    {
        this.Build();
        
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(this._cancellationToken, cancellationToken);
        CancellationToken token = linkedCts.Token;
        
        RequestMetricsBuilder metricsBuilder = new RequestMetricsBuilder();
        int attemptNumber = 0;
        System.Exception? lastException = null;

        foreach (Interfaces.IRequestInterceptor interceptor in this._requestInterceptors.OrderBy(i => i.Order))
        {
            await interceptor.OnRequestAsync(this, token);
        }

        if (this._hostCheckConfig != null)
        {
            await this.CheckHostReachabilityAsync(token);
        }

        while (attemptNumber <= this._retryPolicy.MaxRetries)
        {
            attemptNumber++;
            
            if (attemptNumber > 1)
            {
                metricsBuilder.IncrementRetryCount();
                TimeSpan delay = this._retryPolicy.GetDelay(attemptNumber - 1);
                await Task.Delay(delay, token);
            }

            try
            {
                HttpClient client = this.GetOrCreateClient();
                
                using CancellationTokenSource timeoutCts = new CancellationTokenSource(this._timeout);
                using CancellationTokenSource combinedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token);

                metricsBuilder.SetProxy(this._proxy != null, this._proxy?.Key);

                using HttpRequestMessage httpRequest = this.ToHttpRequestMessage();
                httpRequest.Version = this._httpVersion;
                httpRequest.VersionPolicy = this._httpVersionPolicy;

                metricsBuilder.MarkConnectStart();
                HttpResponseMessage httpResponse = await client.SendAsync(httpRequest, 
                    HttpCompletionOption.ResponseHeadersRead, combinedCts.Token);
                metricsBuilder.MarkConnectEnd();
                metricsBuilder.MarkFirstByte();

                if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    RateLimitException rateLimitException = this.HandleRateLimitResponse(httpResponse);
                    
                    if (attemptNumber > this._retryPolicy.MaxRetries)
                        throw rateLimitException;
                    
                    lastException = rateLimitException;
                    continue;
                }

                MemoryStream contentStream = new MemoryStream();
                await httpResponse.Content.CopyToAsync(contentStream, combinedCts.Token);
                metricsBuilder.MarkDownloadEnd();
                metricsBuilder.SetBytesReceived(contentStream.Length);
                
                StringBuilder sb = StringBuilderPool.Acquire(64);
                sb.Append(HttpConstants.ProtocolHttpPrefix.Span);
                sb.Append(httpResponse.Version);
                metricsBuilder.SetProtocol(sb.ToStringAndRelease());

                this._proxy?.ReportSuccess();

                Response response = new Response(httpResponse, contentStream, metricsBuilder.Build())
                {
                    RequestUri = new Uri(this.Uri.ToString()),
                    FromCache = false
                };

                foreach (Interfaces.IResponseInterceptor interceptor in this._responseInterceptors.OrderBy(i => i.Order))
                {
                    await interceptor.OnResponseAsync(response, token);
                }

                return response;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                lastException = new RequestTimeoutException(this._timeout, new Uri(this.Uri.ToString()), attemptNumber);
                
                if (attemptNumber > this._retryPolicy.MaxRetries)
                    throw lastException;
            }
            catch (HttpRequestException ex) when (IsProxyError(ex))
            {
                this._proxy?.ReportFailure();
                lastException = new ProxyException(ex.Message, ex, this._proxy?.Proxy, attemptNumber);
                
                if (attemptNumber > this._retryPolicy.MaxRetries)
                    throw lastException;
            }
            catch (HttpRequestException ex)
            {
                string uri = this.Uri.ToString();
                string host = new Uri(uri).Host;
                lastException = new NetworkException(ex.Message, ex, host, attemptNumber);
                
                if (attemptNumber > this._retryPolicy.MaxRetries)
                    throw lastException;
            }
            catch (System.Exception ex)
            {
                lastException = ex;
                throw;
            }
        }

        throw lastException ?? (System.Exception)new NetworkException("Request failed after all retries");
    }

    public HttpRequestMessage ToHttpRequestMessage()
    {
        Uri uri = new Uri(this.Uri.ToString());
        HttpRequestMessage message = new HttpRequestMessage(this._method, uri);

        if (this._requestBuilder.RequestHeaderBuilder != null)
        {
            foreach (KeyValuePair<string, string> entry in this._requestBuilder.RequestHeaderBuilder.GetEntries())
            {
                message.Headers.TryAddWithoutValidation(entry.Key, entry.Value);
            }
        }

        if (!this.Body.IsEmpty)
        {
            byte[] bodyArray = this.Body.ToArray();
            message.Content = new ByteArrayContent(bodyArray);
            
            if (this._requestBuilder.RequestHeaderBuilder?.GetHeader("Content-Type") == null)
            {
                if (this._formBuilder != null)
                {
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(
                        $"multipart/form-data; boundary={this._formBuilder.BoundaryString}");
                }
                else if (SharedMimeDictionary.TryGetMimeTypeAsString("json", out string jsonMime))
                {
                    message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonMime);
                }
            }
        }

        return message;
    }

    private HttpClient GetOrCreateClient()
    {
        string key = this.BuildClientKey();

        lock (PoolLock)
        {
            if (ClientPool.TryGetValue(key, out HttpClient? existingClient))
                return existingClient;

            SocketsHttpHandler handler = this.CreateHandler();
            HttpClient client = new HttpClient(handler, disposeHandler: true);
            
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

            ClientPool[key] = client;
            return client;
        }
    }

    private string BuildClientKey()
    {
        StringBuilder sb = StringBuilderPool.Acquire(128);
        
        if (this._proxy != null)
            sb.Append(this._proxy.Key);
        else
            sb.Append(HttpConstants.DirectProxyKey.Span);
            
        sb.Append("|validate:");
        sb.Append(this._validateCertificates);
        sb.Append("|redirect:");
        sb.Append(this._followRedirects);
        sb.Append("|httpver:");
        sb.Append(this._httpVersion);
        
        return sb.ToStringAndRelease();
    }

    private SocketsHttpHandler CreateHandler()
    {
        SocketsHttpHandler handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = PooledConnectionLifetime,
            PooledConnectionIdleTimeout = PooledConnectionIdleTimeout,
            MaxConnectionsPerServer = MaxConnectionsPerServer,
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = this._followRedirects,
            MaxAutomaticRedirections = this._maxRedirects,
            EnableMultipleHttp2Connections = true,
            ConnectTimeout = TimeSpan.FromSeconds(30)
        };

        if (!this._validateCertificates)
        {
            handler.SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            };
        }

        if (this._proxy != null)
        {
            handler.Proxy = this._proxy.ToWebProxy();
            handler.UseProxy = true;
        }

        return handler;
    }

    private async Task CheckHostReachabilityAsync(CancellationToken cancellationToken)
    {
        Uri uri = new Uri(this.Uri.ToString());
        string host = uri.Host;

        if (this._hostCheckConfig!.Method == EnumHostCheckMethod.Ping)
        {
            using Ping ping = new Ping();
            PingReply reply = await ping.SendPingAsync(host, (int)this._hostCheckConfig.Timeout.TotalMilliseconds);
            
            if (reply.Status != IPStatus.Success)
                throw new NetworkException($"Host {host} is not reachable (ping failed: {reply.Status})");
        }
        else
        {
            using TcpClient client = new TcpClient();
            int port = this._hostCheckConfig.Port > 0 ? this._hostCheckConfig.Port : uri.Port;
            Task connectTask = client.ConnectAsync(host, port);
            
            if (await Task.WhenAny(connectTask, Task.Delay(this._hostCheckConfig.Timeout, cancellationToken)) != connectTask)
                throw new NetworkException($"Host {host}:{port} is not reachable (connection timeout)");

            await connectTask;
        }
    }

    private static bool IsProxyError(HttpRequestException ex)
    {
        string message = ex.Message.ToLowerInvariant();
        return message.Contains("proxy") || 
               message.Contains("407") ||
               ex.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
    }

    private void ApplyScrapingBypassIfConfigured(List<KeyValuePair<string, string>>? userHeaders, string? userDefinedUserAgent)
    {
        ScrapingBypassConfig? config = this._scrapingBypass;
        if (config == null)
            return;

        if (config.BrowserProfile != EnumBrowserProfile.None)
        {
            BrowserSpoofing.ApplyBrowserProfile(this, config.BrowserProfile);
        }

        if (config.RefererStrategy != EnumRefererStrategy.None)
        {
            BrowserSpoofing.ApplyRefererStrategy(this, config.RefererStrategy);
        }

        if (userHeaders != null && userHeaders.Count > 0)
        {
            foreach (KeyValuePair<string, string> header in userHeaders)
            {
                this._requestBuilder.RequestHeaderBuilder!.SetHeader(header.Key, header.Value);
            }
        }
        
        // Re-apply user-defined User-Agent after browser spoofing (priority: user > spoofing)
        // This handles WithUserAgent(), WithBogusUserAgent(), and WithBogusUserAgent<T>()
        if (!string.IsNullOrEmpty(userDefinedUserAgent))
        {
            this.WithUserAgent(userDefinedUserAgent);
        }
    }

    private RateLimitException HandleRateLimitResponse(HttpResponseMessage response)
    {
        Uri requestUri = new Uri(this.Uri.ToString());
        
        if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string>? retryAfterValues))
        {
            string? retryAfter = retryAfterValues.FirstOrDefault();
            if (int.TryParse(retryAfter, out int seconds))
                return RateLimitException.FromRetryAfter(seconds, requestUri);
            
            if (DateTime.TryParse(retryAfter, out DateTime dateTime))
                return RateLimitException.FromResetTime(dateTime.ToUniversalTime(), requestUri);
        }

        if (response.Headers.TryGetValues("X-RateLimit-Reset", out IEnumerable<string>? resetValues))
        {
            string? resetStr = resetValues.FirstOrDefault();
            if (long.TryParse(resetStr, out long unixTime))
            {
                DateTime resetTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
                return RateLimitException.FromResetTime(resetTime, requestUri);
            }
        }

        return new RateLimitException("Rate limited", TimeSpan.FromSeconds(60), null, requestUri);
    }

    public static void ConfigureConnectionPool(
        TimeSpan? connectionLifetime = null,
        TimeSpan? connectionIdleTimeout = null,
        int? maxConnections = null)
    {
        if (connectionLifetime.HasValue)
            PooledConnectionLifetime = connectionLifetime.Value;
        if (connectionIdleTimeout.HasValue)
            PooledConnectionIdleTimeout = connectionIdleTimeout.Value;
        if (maxConnections.HasValue)
            MaxConnectionsPerServer = maxConnections.Value;
    }

    public static void ClearClientPool()
    {
        lock (PoolLock)
        {
            foreach (HttpClient client in ClientPool.Values)
                client.Dispose();
            ClientPool.Clear();
        }
    }

}
