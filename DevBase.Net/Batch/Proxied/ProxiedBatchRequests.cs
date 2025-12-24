using System.Collections.Concurrent;
using System.Net;
using System.Text;
using DevBase.Net.Batch.Strategies;
using DevBase.Net.Core;
using DevBase.Net.Proxy;
using DevBase.Net.Utils;

namespace DevBase.Net.Batch.Proxied;

public sealed class ProxiedBatchRequests : IDisposable, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, ProxiedBatch> _batches = new();
    private readonly ConcurrentQueue<Response> _responseQueue = new();
    private readonly ConcurrentDictionary<string, CookieContainer> _cookies = new();
    private readonly ConcurrentDictionary<string, string> _referers = new();
    private readonly List<Func<Response, Task>> _responseCallbacks = new List<Func<Response, Task>>();
    private readonly List<Func<Request, System.Exception, Task>> _errorCallbacks = new List<Func<Request, System.Exception, Task>>();
    private readonly List<Func<BatchProgressInfo, Task>> _progressCallbacks = new List<Func<BatchProgressInfo, Task>>();
    private readonly List<Func<Response, Request, RequeueDecision>> _requeueOnResponseCallbacks = new List<Func<Response, Request, RequeueDecision>>();
    private readonly List<Func<Request, System.Exception, RequeueDecision>> _requeueOnErrorCallbacks = new List<Func<Request, System.Exception, RequeueDecision>>();
    private readonly List<Func<TrackedProxyInfo, ProxyFailureContext, Task>> _proxyFailureCallbacks = new List<Func<TrackedProxyInfo, ProxyFailureContext, Task>>();
    private readonly SemaphoreSlim _rateLimitSemaphore = new(1, 1);
    private readonly SemaphoreSlim _proxyLock = new(1, 1);

    private readonly List<TrackedProxyInfo> _proxyPool = new List<TrackedProxyInfo>();
    private int _currentProxyIndex;
    private IProxyRotationStrategy _rotationStrategy = new RoundRobinStrategy();

    private int _rateLimit = 1;
    private TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(1);
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
    private int _proxyFailureCount;

    public int BatchCount => _batches.Count;
    public int TotalQueueCount => _batches.Values.Sum(b => b.QueueCount);
    public int ResponseQueueCount => _responseQueue.Count;
    public int RateLimit => _rateLimit;
    public bool PersistCookies => _persistCookies;
    public bool PersistReferer => _persistReferer;
    public bool IsProcessing => _isProcessing;
    public int ProcessedCount => _processedCount;
    public int ErrorCount => _errorCount;
    public int ProxyFailureCount => _proxyFailureCount;
    public int ProxyCount => _proxyPool.Count;
    public int AvailableProxyCount => _proxyPool.Count(p => p.IsAvailable());
    public IReadOnlyList<string> BatchNames => _batches.Keys.ToList();

    public ProxiedBatchRequests()
    {
        StartProcessing();
    }

    #region Proxy Configuration

    public ProxiedBatchRequests WithProxy(ProxyInfo proxy)
    {
        ArgumentNullException.ThrowIfNull(proxy);
        _proxyPool.Add(new TrackedProxyInfo(proxy));
        return this;
    }

    public ProxiedBatchRequests WithProxy(string proxyString)
    {
        return WithProxy(ProxyInfo.Parse(proxyString));
    }

    public ProxiedBatchRequests WithProxies(IEnumerable<ProxyInfo> proxies)
    {
        foreach (ProxyInfo proxy in proxies)
            WithProxy(proxy);
        return this;
    }

    public ProxiedBatchRequests WithProxies(IEnumerable<string> proxyStrings)
    {
        foreach (string proxyString in proxyStrings)
            WithProxy(proxyString);
        return this;
    }

    public ProxiedBatchRequests WithRotationStrategy(IProxyRotationStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);
        _rotationStrategy = strategy;
        return this;
    }

    public ProxiedBatchRequests WithRoundRobinRotation()
    {
        _rotationStrategy = new RoundRobinStrategy();
        return this;
    }

    public ProxiedBatchRequests WithRandomRotation()
    {
        _rotationStrategy = new RandomStrategy();
        return this;
    }

    public ProxiedBatchRequests WithLeastFailuresRotation()
    {
        _rotationStrategy = new LeastFailuresStrategy();
        return this;
    }

    public ProxiedBatchRequests WithStickyRotation()
    {
        _rotationStrategy = new StickyStrategy();
        return this;
    }

    public ProxiedBatchRequests ConfigureProxyTracking(int maxFailures = 3, TimeSpan? timeoutDuration = null)
    {
        foreach (TrackedProxyInfo proxy in _proxyPool)
        {
            proxy.ResetTimeout();
        }
        return this;
    }

    #endregion

    #region Configuration

    public ProxiedBatchRequests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestsPerWindow);
        _rateLimit = requestsPerWindow;
        _rateLimitWindow = window ?? TimeSpan.FromSeconds(1);
        return this;
    }

    public ProxiedBatchRequests WithCookiePersistence(bool persist = true)
    {
        _persistCookies = persist;
        return this;
    }

    public ProxiedBatchRequests WithRefererPersistence(bool persist = true)
    {
        _persistReferer = persist;
        return this;
    }

    #endregion

    #region Batch Management

    public ProxiedBatch CreateBatch(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        ProxiedBatch batch = new ProxiedBatch(name, this);
        if (!_batches.TryAdd(name, batch))
            throw new InvalidOperationException($"Batch '{name}' already exists");

        return batch;
    }

    public ProxiedBatch GetOrCreateBatch(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _batches.GetOrAdd(name, n => new ProxiedBatch(n, this));
    }

    public ProxiedBatch? GetBatch(string name)
    {
        return _batches.TryGetValue(name, out ProxiedBatch? batch) ? batch : null;
    }

    public bool RemoveBatch(string name)
    {
        return _batches.TryRemove(name, out _);
    }

    public ProxiedBatchRequests ClearAllBatches()
    {
        foreach (ProxiedBatch batch in _batches.Values)
            batch.Clear();
        return this;
    }

    #endregion

    #region Callbacks

    public ProxiedBatchRequests OnResponse(Func<Response, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _responseCallbacks.Add(callback);
        return this;
    }

    public ProxiedBatchRequests OnResponse(Action<Response> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _responseCallbacks.Add(r => { callback(r); return Task.CompletedTask; });
        return this;
    }

    public ProxiedBatchRequests OnError(Func<Request, System.Exception, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _errorCallbacks.Add(callback);
        return this;
    }

    public ProxiedBatchRequests OnError(Action<Request, System.Exception> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _errorCallbacks.Add((r, e) => { callback(r, e); return Task.CompletedTask; });
        return this;
    }

    public ProxiedBatchRequests OnProgress(Func<BatchProgressInfo, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _progressCallbacks.Add(callback);
        return this;
    }

    public ProxiedBatchRequests OnProgress(Action<BatchProgressInfo> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _progressCallbacks.Add(p => { callback(p); return Task.CompletedTask; });
        return this;
    }

    public ProxiedBatchRequests OnResponseRequeue(Func<Response, Request, RequeueDecision> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _requeueOnResponseCallbacks.Add(callback);
        return this;
    }

    public ProxiedBatchRequests OnErrorRequeue(Func<Request, System.Exception, RequeueDecision> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _requeueOnErrorCallbacks.Add(callback);
        return this;
    }

    public ProxiedBatchRequests OnProxyFailure(Func<TrackedProxyInfo, ProxyFailureContext, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _proxyFailureCallbacks.Add(callback);
        return this;
    }

    public ProxiedBatchRequests OnProxyFailure(Action<TrackedProxyInfo, ProxyFailureContext> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _proxyFailureCallbacks.Add((p, c) => { callback(p, c); return Task.CompletedTask; });
        return this;
    }

    #endregion

    #region Response Queue

    public bool TryDequeueResponse(out Response? response)
    {
        return _responseQueue.TryDequeue(out response);
    }

    public List<Response> DequeueAllResponses()
    {
        List<Response> responses = new List<Response>(_responseQueue.Count);
        while (_responseQueue.TryDequeue(out Response? response))
            responses.Add(response);
        return responses;
    }

    #endregion

    #region Processing

    public void StartProcessing()
    {
        if (_isProcessing)
            return;

        _processingCts = new CancellationTokenSource();
        _isProcessing = true;
        _processingTask = Task.Run(() => ProcessAllBatchesAsync(_processingCts.Token));
    }

    public async Task StopProcessingAsync()
    {
        if (!_isProcessing)
            return;

        _processingCts?.Cancel();

        if (_processingTask != null)
        {
            try
            {
                await _processingTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        _isProcessing = false;
        _processingCts?.Dispose();
        _processingCts = null;
        _processingTask = null;
    }

    public async Task<List<Response>> ExecuteAllAsync(CancellationToken cancellationToken = default)
    {
        ConcurrentBag<Response> responses = new ConcurrentBag<Response>();

        List<(Request Request, string BatchName)> allRequests = CollectAllRequests();
        if (allRequests.Count == 0)
            return new List<Response>();

        int totalRequests = allRequests.Count;
        int completed = 0;
        int index = 0;

        while (index < allRequests.Count && !cancellationToken.IsCancellationRequested)
        {
            int batchSize = Math.Min(_rateLimit, allRequests.Count - index);

            await EnforceRateLimitAsync(cancellationToken).ConfigureAwait(false);

            Task[] tasks = new Task[batchSize];
            int[] completedHolder = new int[] { completed };
            for (int i = 0; i < batchSize; i++)
            {
                (Request Request, string BatchName) item = allRequests[index + i];
                tasks[i] = ProcessProxiedRequestAsync(item.Request, item.BatchName, responses, completedHolder, totalRequests, cancellationToken);
            }
            index += batchSize;

            await Task.WhenAll(tasks).ConfigureAwait(false);
            completed = completedHolder[0];
        }

        return new List<Response>(responses);
    }

    private async Task ProcessProxiedRequestAsync(Request request, string batchName, ConcurrentBag<Response> responses, int[] completedHolder, int totalRequests, CancellationToken cancellationToken)
    {
        TrackedProxyInfo? usedProxy = null;
        try
        {
            ApplyPersistence(request);
            usedProxy = await ApplyProxyAsync(request, cancellationToken).ConfigureAwait(false);

            Response response = await request.SendAsync(cancellationToken).ConfigureAwait(false);

            StorePersistence(request, response);
            usedProxy?.ReportSuccess();

            responses.Add(response);
            _responseQueue.Enqueue(response);
            Interlocked.Increment(ref _processedCount);
            int currentCompleted = Interlocked.Increment(ref completedHolder[0]);

            await InvokeCallbacksAsync(response).ConfigureAwait(false);
            await InvokeProgressCallbacksAsync(new BatchProgressInfo(
                batchName, currentCompleted, totalRequests, _errorCount)).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateResponseRequeue(response, request);
            if (requeueDecision.ShouldRequeue)
            {
                ProxiedBatch? batch = GetBatch(batchName);
                batch?.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (System.Exception ex)
        {
            Interlocked.Increment(ref _errorCount);
            await HandleProxyFailureAsync(usedProxy, ex, request).ConfigureAwait(false);
            await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
            if (requeueDecision.ShouldRequeue)
            {
                ProxiedBatch? batch = GetBatch(batchName);
                batch?.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
    }

    public async Task<List<Response>> ExecuteBatchAsync(string batchName, CancellationToken cancellationToken = default)
    {
        ProxiedBatch? batch = GetBatch(batchName);
        if (batch == null)
            throw new InvalidOperationException($"Batch '{batchName}' not found");

        return await ExecuteBatchInternalAsync(batch, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<Response> ExecuteAllAsyncEnumerable(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<(Request Request, string BatchName)> allRequests = CollectAllRequests();
        int totalRequests = allRequests.Count;
        int completed = 0;
        int index = 0;

        while (index < allRequests.Count && !cancellationToken.IsCancellationRequested)
        {
            int batchSize = Math.Min(_rateLimit, allRequests.Count - index);

            await EnforceRateLimitAsync(cancellationToken).ConfigureAwait(false);

            ConcurrentBag<Response> responseBag = new ConcurrentBag<Response>();
            Task[] tasks = new Task[batchSize];
            int[] completedHolder = new int[] { completed };

            for (int i = 0; i < batchSize; i++)
            {
                (Request Request, string BatchName) item = allRequests[index + i];
                tasks[i] = ProcessEnumerableProxiedRequestAsync(item.Request, item.BatchName, responseBag, completedHolder, totalRequests, cancellationToken);
            }
            index += batchSize;

            await Task.WhenAll(tasks).ConfigureAwait(false);
            completed = completedHolder[0];

            foreach (Response response in responseBag)
                yield return response;
        }
    }

    private async Task ProcessEnumerableProxiedRequestAsync(Request request, string batchName, ConcurrentBag<Response> responseBag, int[] completedHolder, int totalRequests, CancellationToken cancellationToken)
    {
        TrackedProxyInfo? usedProxy = null;
        try
        {
            ApplyPersistence(request);
            usedProxy = await ApplyProxyAsync(request, cancellationToken).ConfigureAwait(false);

            Response response = await request.SendAsync(cancellationToken).ConfigureAwait(false);

            StorePersistence(request, response);
            usedProxy?.ReportSuccess();

            responseBag.Add(response);
            Interlocked.Increment(ref _processedCount);
            int currentCompleted = Interlocked.Increment(ref completedHolder[0]);

            await InvokeCallbacksAsync(response).ConfigureAwait(false);
            await InvokeProgressCallbacksAsync(new BatchProgressInfo(
                batchName, currentCompleted, totalRequests, _errorCount)).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateResponseRequeue(response, request);
            if (requeueDecision.ShouldRequeue)
            {
                ProxiedBatch? batch = GetBatch(batchName);
                batch?.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (System.Exception ex)
        {
            Interlocked.Increment(ref _errorCount);
            await HandleProxyFailureAsync(usedProxy, ex, request).ConfigureAwait(false);
            await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
            if (requeueDecision.ShouldRequeue)
            {
                ProxiedBatch? batch = GetBatch(batchName);
                batch?.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
    }

    private async Task ProcessAllBatchesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Request? request = TryDequeueNextRequest(out string? batchName);
            if (request == null)
            {
                await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                continue;
            }

            TrackedProxyInfo? usedProxy = null;
            try
            {
                ApplyPersistence(request);
                usedProxy = await ApplyProxyAsync(request, cancellationToken).ConfigureAwait(false);

                Response response = await SendWithRateLimitAsync(request, cancellationToken).ConfigureAwait(false);

                StorePersistence(request, response);
                usedProxy?.ReportSuccess();

                _responseQueue.Enqueue(response);
                Interlocked.Increment(ref _processedCount);

                await InvokeCallbacksAsync(response).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (System.Exception ex)
            {
                Interlocked.Increment(ref _errorCount);
                await HandleProxyFailureAsync(usedProxy, ex, request).ConfigureAwait(false);
                await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

                RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
                if (requeueDecision.ShouldRequeue && batchName != null)
                {
                    ProxiedBatch? batch = GetBatch(batchName);
                    batch?.Add(requeueDecision.ModifiedRequest ?? request);
                }
            }
        }
    }

    private async Task<List<Response>> ExecuteBatchInternalAsync(ProxiedBatch batch, CancellationToken cancellationToken)
    {
        ConcurrentBag<Response> responses = new ConcurrentBag<Response>();
        List<Request> requests = batch.DequeueAll();

        if (requests.Count == 0)
            return new List<Response>();

        int totalRequests = requests.Count;
        int completed = 0;
        int index = 0;

        while (index < requests.Count && !cancellationToken.IsCancellationRequested)
        {
            int batchSize = Math.Min(_rateLimit, requests.Count - index);

            await EnforceRateLimitAsync(cancellationToken).ConfigureAwait(false);

            Task[] tasks = new Task[batchSize];
            int[] completedHolder = new int[] { completed };
            for (int i = 0; i < batchSize; i++)
            {
                Request request = requests[index + i];
                tasks[i] = ProcessBatchProxiedRequestAsync(request, batch, responses, completedHolder, totalRequests, cancellationToken);
            }
            index += batchSize;

            await Task.WhenAll(tasks).ConfigureAwait(false);
            completed = completedHolder[0];
        }

        return new List<Response>(responses);
    }

    private async Task ProcessBatchProxiedRequestAsync(Request request, ProxiedBatch batch, ConcurrentBag<Response> responses, int[] completedHolder, int totalRequests, CancellationToken cancellationToken)
    {
        TrackedProxyInfo? usedProxy = null;
        try
        {
            ApplyPersistence(request);
            usedProxy = await ApplyProxyAsync(request, cancellationToken).ConfigureAwait(false);

            Response response = await request.SendAsync(cancellationToken).ConfigureAwait(false);

            StorePersistence(request, response);
            usedProxy?.ReportSuccess();

            responses.Add(response);
            _responseQueue.Enqueue(response);
            Interlocked.Increment(ref _processedCount);
            int currentCompleted = Interlocked.Increment(ref completedHolder[0]);

            await InvokeCallbacksAsync(response).ConfigureAwait(false);
            await InvokeProgressCallbacksAsync(new BatchProgressInfo(
                batch.Name, currentCompleted, totalRequests, _errorCount)).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateResponseRequeue(response, request);
            if (requeueDecision.ShouldRequeue)
            {
                batch.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (System.Exception ex)
        {
            Interlocked.Increment(ref _errorCount);
            await HandleProxyFailureAsync(usedProxy, ex, request).ConfigureAwait(false);
            await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
            if (requeueDecision.ShouldRequeue)
            {
                batch.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
    }

    private List<(Request Request, string BatchName)> CollectAllRequests()
    {
        List<(Request, string)> allRequests = new List<(Request, string)>();
        foreach (ProxiedBatch batch in _batches.Values)
        {
            List<Request> requests = batch.DequeueAll();
            foreach (Request request in requests)
            {
                allRequests.Add((request, batch.Name));
            }
        }
        return allRequests;
    }

    private Request? TryDequeueNextRequest(out string? batchName)
    {
        foreach (ProxiedBatch batch in _batches.Values)
        {
            if (batch.TryDequeue(out Request? request))
            {
                batchName = batch.Name;
                return request;
            }
        }
        batchName = null;
        return null;
    }

    #endregion

    #region Proxy Rotation

    private async Task<TrackedProxyInfo?> ApplyProxyAsync(Request request, CancellationToken cancellationToken)
    {
        if (_proxyPool.Count == 0)
            return null;

        await _proxyLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            TrackedProxyInfo? proxy = _rotationStrategy.SelectProxy(_proxyPool, ref _currentProxyIndex);
            if (proxy != null)
            {
                request.WithProxy(proxy.Proxy);
            }
            return proxy;
        }
        finally
        {
            _proxyLock.Release();
        }
    }

    private async Task HandleProxyFailureAsync(TrackedProxyInfo? proxy, System.Exception exception, Request request)
    {
        if (proxy == null)
            return;

        bool timedOut = proxy.ReportFailure();
        Interlocked.Increment(ref _proxyFailureCount);

        ProxyFailureContext context = new ProxyFailureContext(
            proxy,
            exception,
            request,
            timedOut,
            proxy.FailureCount,
            _proxyPool.Count(p => p.IsAvailable())
        );

        for (int i = 0; i < _proxyFailureCallbacks.Count; i++)
        {
            try
            {
                await _proxyFailureCallbacks[i](proxy, context).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    public IReadOnlyList<TrackedProxyInfo> GetProxyStatistics()
    {
        return _proxyPool.AsReadOnly();
    }

    public void ResetAllProxies()
    {
        foreach (TrackedProxyInfo proxy in _proxyPool)
            proxy.ResetTimeout();
    }

    public void ClearProxies()
    {
        _proxyPool.Clear();
        _currentProxyIndex = 0;
    }

    #endregion

    #region Rate Limiting

    private async Task<Response> SendWithRateLimitAsync(Request request, CancellationToken cancellationToken)
    {
        await EnforceRateLimitAsync(cancellationToken).ConfigureAwait(false);
        return await request.SendAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task EnforceRateLimitAsync(CancellationToken cancellationToken)
    {
        if (_rateLimit <= 0)
            return;

        await _rateLimitSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            DateTime now = DateTime.UtcNow;

            if (now - _windowStart >= _rateLimitWindow)
            {
                _windowStart = now;
                _requestsInWindow = 0;
            }

            if (_requestsInWindow >= _rateLimit)
            {
                TimeSpan waitTime = _rateLimitWindow - (now - _windowStart);
                if (waitTime > TimeSpan.Zero)
                    await Task.Delay(waitTime, cancellationToken).ConfigureAwait(false);

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

    #endregion

    #region Persistence

    private void ApplyPersistence(Request request)
    {
        Uri? uri = request.GetUri();
        if (uri == null)
            return;

        string host = uri.Host;

        if (_persistCookies && _cookies.TryGetValue(host, out CookieContainer? container))
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

        if (_persistReferer && _referers.TryGetValue(host, out string? referer))
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

        if (_persistCookies)
        {
            CookieCollection responseCookies = response.GetCookies();
            if (responseCookies.Count > 0)
            {
                CookieContainer container = _cookies.GetOrAdd(host, _ => new CookieContainer());
                foreach (Cookie cookie in responseCookies)
                    container.Add(uri, cookie);
            }
        }

        if (_persistReferer)
        {
            _referers[host] = uri.ToString();
        }
    }

    #endregion

    #region Callbacks

    private async Task InvokeCallbacksAsync(Response response)
    {
        for (int i = 0; i < _responseCallbacks.Count; i++)
        {
            try
            {
                await _responseCallbacks[i](response).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private async Task InvokeErrorCallbacksAsync(Request request, System.Exception exception)
    {
        for (int i = 0; i < _errorCallbacks.Count; i++)
        {
            try
            {
                await _errorCallbacks[i](request, exception).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private async Task InvokeProgressCallbacksAsync(BatchProgressInfo progress)
    {
        for (int i = 0; i < _progressCallbacks.Count; i++)
        {
            try
            {
                await _progressCallbacks[i](progress).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private RequeueDecision EvaluateResponseRequeue(Response response, Request request)
    {
        for (int i = 0; i < _requeueOnResponseCallbacks.Count; i++)
        {
            try
            {
                RequeueDecision decision = _requeueOnResponseCallbacks[i](response, request);
                if (decision.ShouldRequeue)
                    return decision;
            }
            catch
            {
            }
        }
        return RequeueDecision.NoRequeue;
    }

    private RequeueDecision EvaluateErrorRequeue(Request request, System.Exception exception)
    {
        for (int i = 0; i < _requeueOnErrorCallbacks.Count; i++)
        {
            try
            {
                RequeueDecision decision = _requeueOnErrorCallbacks[i](request, exception);
                if (decision.ShouldRequeue)
                    return decision;
            }
            catch
            {
            }
        }
        return RequeueDecision.NoRequeue;
    }

    #endregion

    #region Utilities

    public void ResetCounters()
    {
        Interlocked.Exchange(ref _processedCount, 0);
        Interlocked.Exchange(ref _errorCount, 0);
        Interlocked.Exchange(ref _proxyFailureCount, 0);
    }

    public ProxiedBatchStatistics GetStatistics()
    {
        return new ProxiedBatchStatistics(
            BatchCount,
            TotalQueueCount,
            ProcessedCount,
            ErrorCount,
            ProxyFailureCount,
            ProxyCount,
            AvailableProxyCount,
            _batches.ToDictionary(b => b.Key, b => b.Value.QueueCount)
        );
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _processingCts?.Cancel();
        _processingCts?.Dispose();
        _rateLimitSemaphore.Dispose();
        _proxyLock.Dispose();

        foreach (ProxiedBatch batch in _batches.Values)
            batch.Clear();

        _batches.Clear();
        _cookies.Clear();
        _referers.Clear();
        _responseCallbacks.Clear();
        _errorCallbacks.Clear();
        _progressCallbacks.Clear();
        _proxyPool.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await StopProcessingAsync();
        Dispose();
    }

    #endregion
}
