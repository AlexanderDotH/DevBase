using System.Collections.Concurrent;
using System.Net;
using System.Threading.Channels;
using DevBase.Requests.Configuration;

namespace DevBase.Requests;

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

    public int QueueCount => _queue.Count;
    public int RateLimit => _rateLimit;
    public int Parallelism => _parallelism;
    public bool PersistCookies => _persistCookies;
    public bool PersistReferer => _persistReferer;

    public Requests()
    {
        _rateLimitSemaphore = new SemaphoreSlim(1, 1);
    }

    #region Configuration

    public Requests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestsPerWindow);
        _rateLimit = requestsPerWindow;
        _rateLimitWindow = window ?? TimeSpan.FromSeconds(1);
        return this;
    }

    public Requests WithParallelism(int maxParallel)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxParallel);
        _parallelism = maxParallel;
        return this;
    }

    public Requests WithCookiePersistence(bool persist = true)
    {
        _persistCookies = persist;
        return this;
    }

    public Requests WithRefererPersistence(bool persist = true)
    {
        _persistReferer = persist;
        return this;
    }

    #endregion

    #region Queue Management

    public Requests Add(Request request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _queue.Enqueue(request);
        return this;
    }

    public Requests Add(IEnumerable<Request> requests)
    {
        foreach (var request in requests)
            Add(request);
        return this;
    }

    public Requests Add(string url)
    {
        return Add(new Request(url));
    }

    public Requests Add(IEnumerable<string> urls)
    {
        foreach (var url in urls)
            Add(url);
        return this;
    }

    public void Clear()
    {
        while (_queue.TryDequeue(out _)) { }
    }

    #endregion

    #region Callbacks

    public Requests OnResponse(Func<Response, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _responseCallbacks.Add(callback);
        return this;
    }

    public Requests OnResponse(Action<Response> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _responseCallbacks.Add(r => { callback(r); return Task.CompletedTask; });
        return this;
    }

    #endregion

    #region Execution

    public async Task<List<Response>> SendAllAsync(CancellationToken cancellationToken = default)
    {
        var responses = new ConcurrentBag<Response>();
        var requests = new List<Request>();

        while (_queue.TryDequeue(out var request))
            requests.Add(request);

        if (_parallelism <= 1)
        {
            foreach (var request in requests)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var response = await SendWithRateLimitAsync(request, cancellationToken);
                responses.Add(response);
                await InvokeCallbacksAsync(response);
            }
        }
        else
        {
            var channel = Channel.CreateBounded<Request>(new BoundedChannelOptions(_parallelism * 2)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            var producer = Task.Run(async () =>
            {
                foreach (var request in requests)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    await channel.Writer.WriteAsync(request, cancellationToken);
                }
                channel.Writer.Complete();
            }, cancellationToken);

            var consumers = Enumerable.Range(0, _parallelism)
                .Select(_ => Task.Run(async () =>
                {
                    await foreach (var request in channel.Reader.ReadAllAsync(cancellationToken))
                    {
                        try
                        {
                            var response = await SendWithRateLimitAsync(request, cancellationToken);
                            responses.Add(response);
                            await InvokeCallbacksAsync(response);
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
        while (_queue.TryDequeue(out var request))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var response = await SendWithRateLimitAsync(request, cancellationToken);
            await InvokeCallbacksAsync(response);
            yield return response;
        }
    }

    private async Task<Response> SendWithRateLimitAsync(Request request, CancellationToken cancellationToken)
    {
        await EnforceRateLimitAsync(cancellationToken);
        
        ApplyPersistence(request);
        
        var response = await request.SendAsync(cancellationToken);
        
        StorePersistence(request, response);
        
        return response;
    }

    private async Task EnforceRateLimitAsync(CancellationToken cancellationToken)
    {
        if (_rateLimit <= 0)
            return;

        await _rateLimitSemaphore.WaitAsync(cancellationToken);
        try
        {
            var now = DateTime.UtcNow;
            
            if (now - _windowStart >= _rateLimitWindow)
            {
                _windowStart = now;
                _requestsInWindow = 0;
            }

            if (_requestsInWindow >= _rateLimit)
            {
                var waitTime = _rateLimitWindow - (now - _windowStart);
                if (waitTime > TimeSpan.Zero)
                    await Task.Delay(waitTime, cancellationToken);
                
                _windowStart = DateTime.UtcNow;
                _requestsInWindow = 0;
            }

            _requestsInWindow++;
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    private void ApplyPersistence(Request request)
    {
        var uri = request.GetUri();
        if (uri == null)
            return;

        var host = uri.Host;

        if (_persistCookies && _cookies.TryGetValue(host, out var container))
        {
            var cookies = container.GetCookies(uri);
            if (cookies.Count > 0)
            {
                var cookieHeader = string.Join("; ", cookies.Cast<Cookie>().Select(c => $"{c.Name}={c.Value}"));
                request.WithCookie(cookieHeader);
            }
        }

        if (_persistReferer && _referers.TryGetValue(host, out var referer))
        {
            request.WithReferer(referer);
        }
    }

    private void StorePersistence(Request request, Response response)
    {
        var uri = request.GetUri();
        if (uri == null)
            return;

        var host = uri.Host;

        if (_persistCookies)
        {
            var responseCookies = response.GetCookies();
            if (responseCookies.Count > 0)
            {
                var container = _cookies.GetOrAdd(host, _ => new CookieContainer());
                foreach (Cookie cookie in responseCookies)
                    container.Add(uri, cookie);
            }
        }

        if (_persistReferer)
        {
            _referers[host] = uri.ToString();
        }
    }

    private async Task InvokeCallbacksAsync(Response response)
    {
        foreach (var callback in _responseCallbacks)
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

    #endregion

    #region Disposal

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _rateLimitSemaphore.Dispose();
        Clear();
        _cookies.Clear();
        _referers.Clear();
        _responseCallbacks.Clear();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    #endregion
}
