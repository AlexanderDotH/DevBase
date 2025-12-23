using System.Collections.Concurrent;
using System.Net;
using System.Threading.Channels;

namespace DevBase.Requests.Core;

public sealed class Requests : IDisposable, IAsyncDisposable
{
    private readonly ConcurrentQueue<Request> _queue = new();
    private readonly ConcurrentDictionary<string, CookieContainer> _cookies = new();
    private readonly ConcurrentDictionary<string, string> _referers = new();
    private readonly List<Func<Response, Task>> _responseCallbacks = [];
    private readonly SemaphoreSlim _rateLimitSemaphore;
    private readonly object _lock = new();
    
    private int _rateLimit;
    private TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(1);
    private int _parallelism = 1;
    private bool _persistCookies;
    private bool _persistReferer;
    private bool _disposed;
    private DateTime _windowStart = DateTime.UtcNow;
    private int _requestsInWindow;

    public int QueueCount => this._queue.Count;
    public int RateLimit => this._rateLimit;
    public int Parallelism => this._parallelism;
    public bool PersistCookies => this._persistCookies;
    public bool PersistReferer => this._persistReferer;

    public Requests()
    {
        this._rateLimitSemaphore = new SemaphoreSlim(1, 1);
    }

    public Requests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestsPerWindow);
        this._rateLimit = requestsPerWindow;
        this._rateLimitWindow = window ?? TimeSpan.FromSeconds(1);
        return this;
    }

    public Requests WithParallelism(int maxParallel)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxParallel);
        this._parallelism = maxParallel;
        return this;
    }

    public Requests WithCookiePersistence(bool persist = true)
    {
        this._persistCookies = persist;
        return this;
    }

    public Requests WithRefererPersistence(bool persist = true)
    {
        this._persistReferer = persist;
        return this;
    }

    public Requests Add(Request request)
    {
        ArgumentNullException.ThrowIfNull(request);
        this._queue.Enqueue(request);
        return this;
    }

    public Requests Add(IEnumerable<Request> requests)
    {
        foreach (Request request in requests)
            this.Add(request);
        return this;
    }

    public Requests Add(string url)
    {
        return this.Add(new Request(url));
    }

    public Requests Add(IEnumerable<string> urls)
    {
        foreach (string url in urls)
            this.Add(url);
        return this;
    }

    public void Clear()
    {
        while (this._queue.TryDequeue(out _)) { }
    }

    public Requests OnResponse(Func<Response, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        this._responseCallbacks.Add(callback);
        return this;
    }

    public Requests OnResponse(Action<Response> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        this._responseCallbacks.Add(r => { callback(r); return Task.CompletedTask; });
        return this;
    }

    public async Task<List<Response>> SendAllAsync(CancellationToken cancellationToken = default)
    {
        ConcurrentBag<Response> responses = new ConcurrentBag<Response>();
        List<Request> requests = new List<Request>();

        while (this._queue.TryDequeue(out Request? request))
            requests.Add(request);

        if (this._parallelism <= 1)
        {
            foreach (Request request in requests)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                Response response = await this.SendWithRateLimitAsync(request, cancellationToken);
                responses.Add(response);
                await this.InvokeCallbacksAsync(response);
            }
        }
        else
        {
            Channel<Request> channel = Channel.CreateBounded<Request>(new BoundedChannelOptions(this._parallelism * 2)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            Task producer = Task.Run(async () =>
            {
                foreach (Request request in requests)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    await channel.Writer.WriteAsync(request, cancellationToken);
                }
                channel.Writer.Complete();
            }, cancellationToken);

            Task[] consumers = Enumerable.Range(0, this._parallelism)
                .Select(_ => Task.Run(async () =>
                {
                    await foreach (Request request in channel.Reader.ReadAllAsync(cancellationToken))
                    {
                        try
                        {
                            Response response = await this.SendWithRateLimitAsync(request, cancellationToken);
                            responses.Add(response);
                            await this.InvokeCallbacksAsync(response);
                        }
                        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        catch
                        {
                            // Log error but continue processing
                        }
                    }
                }, cancellationToken))
                .ToArray();

            await producer;
            await Task.WhenAll(consumers);
        }

        return responses.ToList();
    }

    public async IAsyncEnumerable<Response> SendAllAsyncEnumerable(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (this._queue.TryDequeue(out Request? request))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            Response response = await this.SendWithRateLimitAsync(request, cancellationToken);
            await this.InvokeCallbacksAsync(response);
            yield return response;
        }
    }

    private async Task<Response> SendWithRateLimitAsync(Request request, CancellationToken cancellationToken)
    {
        await this.EnforceRateLimitAsync(cancellationToken);
        
        this.ApplyPersistence(request);
        
        Response response = await request.SendAsync(cancellationToken);
        
        this.StorePersistence(request, response);
        
        return response;
    }

    private async Task EnforceRateLimitAsync(CancellationToken cancellationToken)
    {
        if (this._rateLimit <= 0)
            return;

        await this._rateLimitSemaphore.WaitAsync(cancellationToken);
        try
        {
            DateTime now = DateTime.UtcNow;
            
            if (now - this._windowStart >= this._rateLimitWindow)
            {
                this._windowStart = now;
                this._requestsInWindow = 0;
            }

            if (this._requestsInWindow >= this._rateLimit)
            {
                TimeSpan waitTime = this._rateLimitWindow - (now - this._windowStart);
                if (waitTime > TimeSpan.Zero)
                    await Task.Delay(waitTime, cancellationToken);
                
                this._windowStart = DateTime.UtcNow;
                this._requestsInWindow = 0;
            }

            this._requestsInWindow++;
        }
        finally
        {
            this._rateLimitSemaphore.Release();
        }
    }

    private void ApplyPersistence(Request request)
    {
        Uri? uri = request.GetUri();
        if (uri == null)
            return;

        string host = uri.Host;

        if (this._persistCookies && this._cookies.TryGetValue(host, out CookieContainer? container))
        {
            CookieCollection cookies = container.GetCookies(uri);
            if (cookies.Count > 0)
            {
                string cookieHeader = string.Join("; ", cookies.Cast<Cookie>().Select(c => $"{c.Name}={c.Value}"));
                request.WithCookie(cookieHeader);
            }
        }

        if (this._persistReferer && this._referers.TryGetValue(host, out string? referer))
        {
            request.WithReferer(referer);
        }
    }

    private void StorePersistence(Request request, Response response)
    {
        Uri? uri = request.GetUri();
        if (uri == null)
            return;

        string host = uri.Host;

        if (this._persistCookies)
        {
            CookieCollection responseCookies = response.GetCookies();
            if (responseCookies.Count > 0)
            {
                CookieContainer container = this._cookies.GetOrAdd(host, _ => new CookieContainer());
                foreach (Cookie cookie in responseCookies)
                    container.Add(uri, cookie);
            }
        }

        if (this._persistReferer)
        {
            this._referers[host] = uri.ToString();
        }
    }

    private async Task InvokeCallbacksAsync(Response response)
    {
        foreach (Func<Response, Task> callback in this._responseCallbacks)
        {
            try
            {
                await callback(response);
            }
            catch
            {
                // Ignore callback errors
            }
        }
    }

    public void Dispose()
    {
        if (this._disposed) return;
        this._disposed = true;

        this._rateLimitSemaphore.Dispose();
        this.Clear();
        this._cookies.Clear();
        this._referers.Clear();
        this._responseCallbacks.Clear();
    }

    public ValueTask DisposeAsync()
    {
        this.Dispose();
        return ValueTask.CompletedTask;
    }
}
