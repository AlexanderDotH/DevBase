using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Threading.Channels;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Core;

public sealed class Requests : IDisposable, IAsyncDisposable
{
    private readonly ConcurrentQueue<Request> _queue = new();
    private readonly ConcurrentQueue<Response> _responseQueue = new();
    private readonly ConcurrentDictionary<string, CookieContainer> _cookies = new();
    private readonly ConcurrentDictionary<string, string> _referers = new();
    private readonly List<Func<Response, Task>> _responseCallbacks = [];
    private readonly List<Func<Request, System.Exception, Task>> _errorCallbacks = [];
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
    
    private CancellationTokenSource? _processingCts;
    private Task? _processingTask;
    private bool _isProcessing;
    private int _processedCount;
    private int _errorCount;

    public int QueueCount => this._queue.Count;
    public int ResponseQueueCount => this._responseQueue.Count;
    public int RateLimit => this._rateLimit;
    public int Parallelism => this._parallelism;
    public bool PersistCookies => this._persistCookies;
    public bool PersistReferer => this._persistReferer;
    public bool IsProcessing => this._isProcessing;
    public int ProcessedCount => this._processedCount;
    public int ErrorCount => this._errorCount;

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

    public Requests Enqueue(Request request) => Add(request);
    public Requests Enqueue(string url) => Add(url);
    public Requests Enqueue(IEnumerable<Request> requests) => Add(requests);
    public Requests Enqueue(IEnumerable<string> urls) => Add(urls);

    public Requests Enqueue(string url, Action<Request> configure)
    {
        Request request = new Request(url);
        configure(request);
        return Add(request);
    }

    public Requests Enqueue(Func<Request> requestFactory)
    {
        return Add(requestFactory());
    }

    public bool TryDequeueResponse(out Response? response)
    {
        return this._responseQueue.TryDequeue(out response);
    }

    public List<Response> DequeueAllResponses()
    {
        List<Response> responses = new();
        while (this._responseQueue.TryDequeue(out Response? response))
            responses.Add(response);
        return responses;
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

    public Requests OnError(Func<Request, System.Exception, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        this._errorCallbacks.Add(callback);
        return this;
    }

    public Requests OnError(Action<Request, System.Exception> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        this._errorCallbacks.Add((r, e) => { callback(r, e); return Task.CompletedTask; });
        return this;
    }

    public void StartProcessing()
    {
        if (this._isProcessing)
            return;

        this._processingCts = new CancellationTokenSource();
        this._isProcessing = true;
        this._processingTask = Task.Run(() => ProcessQueueAsync(this._processingCts.Token));
    }

    public async Task StopProcessingAsync()
    {
        if (!this._isProcessing)
            return;

        this._processingCts?.Cancel();
        
        if (this._processingTask != null)
        {
            try
            {
                await this._processingTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        this._isProcessing = false;
        this._processingCts?.Dispose();
        this._processingCts = null;
        this._processingTask = null;
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (this._queue.TryDequeue(out Request? request))
            {
                try
                {
                    Response response = await this.SendWithRateLimitAsync(request, cancellationToken);
                    this._responseQueue.Enqueue(response);
                    Interlocked.Increment(ref this._processedCount);
                    await this.InvokeCallbacksAsync(response);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (System.Exception ex)
                {
                    Interlocked.Increment(ref this._errorCount);
                    await this.InvokeErrorCallbacksAsync(request, ex);
                }
            }
            else
            {
                await Task.Delay(50, cancellationToken);
            }
        }
    }

    public async Task ProcessUntilEmptyAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested && this._queue.TryDequeue(out Request? request))
        {
            try
            {
                Response response = await this.SendWithRateLimitAsync(request, cancellationToken);
                this._responseQueue.Enqueue(response);
                Interlocked.Increment(ref this._processedCount);
                await this.InvokeCallbacksAsync(response);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (System.Exception ex)
            {
                Interlocked.Increment(ref this._errorCount);
                await this.InvokeErrorCallbacksAsync(request, ex);
            }
        }
    }

    private async Task InvokeErrorCallbacksAsync(Request request, System.Exception exception)
    {
        foreach (Func<Request, System.Exception, Task> callback in this._errorCallbacks)
        {
            try
            {
                await callback(request, exception);
            }
            catch
            {
                // Ignore callback errors
            }
        }
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
                StringBuilder sb = StringBuilderPool.Acquire(512);
                
                for (int i = 0; i < cookies.Count; i++)
                {
                    if (i > 0)
                        sb.Append("; ");
                    
                    Cookie cookie = cookies[i];
                    sb.Append(cookie.Name);
                    sb.Append('=');
                    sb.Append(cookie.Value);
                }
                
                request.WithCookie(sb.ToStringAndRelease());
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

    public void ResetCounters()
    {
        Interlocked.Exchange(ref this._processedCount, 0);
        Interlocked.Exchange(ref this._errorCount, 0);
    }

    public void Dispose()
    {
        if (this._disposed) return;
        this._disposed = true;

        this._processingCts?.Cancel();
        this._processingCts?.Dispose();
        this._rateLimitSemaphore.Dispose();
        this.Clear();
        this._cookies.Clear();
        this._referers.Clear();
        this._responseCallbacks.Clear();
        this._errorCallbacks.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        if (this._disposed) return;
        
        await this.StopProcessingAsync();
        this.Dispose();
    }
}
