using System.Collections.Concurrent;
using System.Net;
using System.Text;
using DevBase.Net.Utils;

namespace DevBase.Net.Core;

public sealed class BatchRequests : IDisposable, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Batch> _batches = new();
    private readonly ConcurrentQueue<Response> _responseQueue = new();
    private readonly ConcurrentDictionary<string, CookieContainer> _cookies = new();
    private readonly ConcurrentDictionary<string, string> _referers = new();
    private readonly List<Func<Response, Task>> _responseCallbacks = [];
    private readonly List<Func<Request, System.Exception, Task>> _errorCallbacks = [];
    private readonly List<Func<BatchProgressInfo, Task>> _progressCallbacks = [];
    private readonly List<Func<Response, Request, RequeueDecision>> _reueueOnResponseCallbacks = [];
    private readonly List<Func<Request, System.Exception, RequeueDecision>> _requeueOnErrorCallbacks = [];
    private readonly SemaphoreSlim _rateLimitSemaphore = new(1, 1);

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

    public int BatchCount => _batches.Count;
    public int TotalQueueCount => _batches.Values.Sum(b => b.QueueCount);
    public int ResponseQueueCount => _responseQueue.Count;
    public int RateLimit => _rateLimit;
    public bool PersistCookies => _persistCookies;
    public bool PersistReferer => _persistReferer;
    public bool IsProcessing => _isProcessing;
    public int ProcessedCount => _processedCount;
    public int ErrorCount => _errorCount;
    public IReadOnlyList<string> BatchNames => _batches.Keys.ToList();

    public BatchRequests()
    {
        StartProcessing();
    }

    #region Configuration

    public BatchRequests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestsPerWindow);
        _rateLimit = requestsPerWindow;
        _rateLimitWindow = window ?? TimeSpan.FromSeconds(1);
        return this;
    }

    public BatchRequests WithCookiePersistence(bool persist = true)
    {
        _persistCookies = persist;
        return this;
    }

    public BatchRequests WithRefererPersistence(bool persist = true)
    {
        _persistReferer = persist;
        return this;
    }

    #endregion

    #region Batch Management

    public Batch CreateBatch(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Batch batch = new Batch(name, this);
        if (!_batches.TryAdd(name, batch))
            throw new InvalidOperationException($"Batch '{name}' already exists");
        
        return batch;
    }

    public Batch GetOrCreateBatch(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _batches.GetOrAdd(name, n => new Batch(n, this));
    }

    public Batch? GetBatch(string name)
    {
        return _batches.TryGetValue(name, out Batch? batch) ? batch : null;
    }

    public bool RemoveBatch(string name)
    {
        return _batches.TryRemove(name, out _);
    }

    public BatchRequests ClearAllBatches()
    {
        foreach (Batch batch in _batches.Values)
            batch.Clear();
        return this;
    }

    #endregion

    #region Callbacks

    public BatchRequests OnResponse(Func<Response, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _responseCallbacks.Add(callback);
        return this;
    }

    public BatchRequests OnResponse(Action<Response> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _responseCallbacks.Add(r => { callback(r); return Task.CompletedTask; });
        return this;
    }

    public BatchRequests OnError(Func<Request, System.Exception, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _errorCallbacks.Add(callback);
        return this;
    }

    public BatchRequests OnError(Action<Request, System.Exception> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _errorCallbacks.Add((r, e) => { callback(r, e); return Task.CompletedTask; });
        return this;
    }

    public BatchRequests OnProgress(Func<BatchProgressInfo, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _progressCallbacks.Add(callback);
        return this;
    }

    public BatchRequests OnProgress(Action<BatchProgressInfo> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _progressCallbacks.Add(p => { callback(p); return Task.CompletedTask; });
        return this;
    }

    public BatchRequests OnResponseRequeue(Func<Response, Request, RequeueDecision> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _reueueOnResponseCallbacks.Add(callback);
        return this;
    }

    public BatchRequests OnErrorRequeue(Func<Request, System.Exception, RequeueDecision> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _requeueOnErrorCallbacks.Add(callback);
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
                tasks[i] = ProcessRequestAsync(item.Request, item.BatchName, responses, completedHolder, totalRequests, cancellationToken);
            }
            index += batchSize;

            await Task.WhenAll(tasks).ConfigureAwait(false);
            completed = completedHolder[0];
        }

        return new List<Response>(responses);
    }

    private async Task ProcessRequestAsync(Request request, string batchName, ConcurrentBag<Response> responses, int[] completedHolder, int totalRequests, CancellationToken cancellationToken)
    {
        try
        {
            ApplyPersistence(request);

            Response response = await request.SendAsync(cancellationToken).ConfigureAwait(false);
            
            StorePersistence(request, response);

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
                Batch? batch = GetBatch(batchName);
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
            await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
            if (requeueDecision.ShouldRequeue)
            {
                Batch? batch = GetBatch(batchName);
                batch?.Add(requeueDecision.ModifiedRequest ?? request);
            }
        }
    }

    public async Task<List<Response>> ExecuteBatchAsync(string batchName, CancellationToken cancellationToken = default)
    {
        Batch? batch = GetBatch(batchName);
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
                tasks[i] = ProcessEnumerableRequestAsync(item.Request, item.BatchName, responseBag, completedHolder, totalRequests, cancellationToken);
            }
            index += batchSize;

            await Task.WhenAll(tasks).ConfigureAwait(false);
            completed = completedHolder[0];

            foreach (Response response in responseBag)
                yield return response;
        }
    }

    private async Task ProcessEnumerableRequestAsync(Request request, string batchName, ConcurrentBag<Response> responseBag, int[] completedHolder, int totalRequests, CancellationToken cancellationToken)
    {
        try
        {
            ApplyPersistence(request);

            Response response = await request.SendAsync(cancellationToken).ConfigureAwait(false);

            StorePersistence(request, response);

            responseBag.Add(response);
            Interlocked.Increment(ref _processedCount);
            int currentCompleted = Interlocked.Increment(ref completedHolder[0]);

            await InvokeCallbacksAsync(response).ConfigureAwait(false);
            await InvokeProgressCallbacksAsync(new BatchProgressInfo(
                batchName, currentCompleted, totalRequests, _errorCount)).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateResponseRequeue(response, request);
            if (requeueDecision.ShouldRequeue)
            {
                Batch? batch = GetBatch(batchName);
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
            await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

            RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
            if (requeueDecision.ShouldRequeue)
            {
                Batch? batch = GetBatch(batchName);
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

            try
            {
                ApplyPersistence(request);

                Response response = await SendWithRateLimitAsync(request, cancellationToken).ConfigureAwait(false);

                StorePersistence(request, response);

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
                await InvokeErrorCallbacksAsync(request, ex).ConfigureAwait(false);

                RequeueDecision requeueDecision = EvaluateErrorRequeue(request, ex);
                if (requeueDecision.ShouldRequeue && batchName != null)
                {
                    Batch? batch = GetBatch(batchName);
                    batch?.Add(requeueDecision.ModifiedRequest ?? request);
                }
            }
        }
    }

    private async Task<List<Response>> ExecuteBatchInternalAsync(Batch batch, CancellationToken cancellationToken)
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
                tasks[i] = ProcessBatchRequestAsync(request, batch, responses, completedHolder, totalRequests, cancellationToken);
            }
            index += batchSize;

            await Task.WhenAll(tasks).ConfigureAwait(false);
            completed = completedHolder[0];
        }

        return new List<Response>(responses);
    }

    private async Task ProcessBatchRequestAsync(Request request, Batch batch, ConcurrentBag<Response> responses, int[] completedHolder, int totalRequests, CancellationToken cancellationToken)
    {
        try
        {
            ApplyPersistence(request);

            Response response = await request.SendAsync(cancellationToken).ConfigureAwait(false);

            StorePersistence(request, response);

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
        foreach (Batch batch in _batches.Values)
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
        foreach (Batch batch in _batches.Values)
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
        for (int i = 0; i < _reueueOnResponseCallbacks.Count; i++)
        {
            try
            {
                RequeueDecision decision = _reueueOnResponseCallbacks[i](response, request);
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
    }

    public BatchStatistics GetStatistics()
    {
        return new BatchStatistics(
            BatchCount,
            TotalQueueCount,
            ProcessedCount,
            ErrorCount,
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

        foreach (Batch batch in _batches.Values)
            batch.Clear();
        
        _batches.Clear();
        _cookies.Clear();
        _referers.Clear();
        _responseCallbacks.Clear();
        _errorCallbacks.Clear();
        _progressCallbacks.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await StopProcessingAsync();
        Dispose();
    }

    #endregion
}

public sealed class Batch
{
    private readonly ConcurrentQueue<Request> _queue = new();
    private readonly BatchRequests _parent;

    public string Name { get; }
    public int QueueCount => _queue.Count;

    internal Batch(string name, BatchRequests parent)
    {
        Name = name;
        _parent = parent;
    }

    public Batch Add(Request request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _queue.Enqueue(request);
        return this;
    }

    public Batch Add(IEnumerable<Request> requests)
    {
        foreach (Request request in requests)
            Add(request);
        return this;
    }

    public Batch Add(string url)
    {
        return Add(new Request(url));
    }

    public Batch Add(IEnumerable<string> urls)
    {
        foreach (string url in urls)
            Add(url);
        return this;
    }

    public Batch Enqueue(Request request) => Add(request);
    public Batch Enqueue(string url) => Add(url);
    public Batch Enqueue(IEnumerable<Request> requests) => Add(requests);
    public Batch Enqueue(IEnumerable<string> urls) => Add(urls);

    public Batch Enqueue(string url, Action<Request> configure)
    {
        Request request = new Request(url);
        configure(request);
        return Add(request);
    }

    public Batch Enqueue(Func<Request> requestFactory)
    {
        return Add(requestFactory());
    }

    public bool TryDequeue(out Request? request)
    {
        return _queue.TryDequeue(out request);
    }

    internal List<Request> DequeueAll()
    {
        List<Request> requests = new List<Request>(_queue.Count);
        while (_queue.TryDequeue(out Request? request))
            requests.Add(request);
        return requests;
    }

    public void Clear()
    {
        while (_queue.TryDequeue(out _)) { }
    }

    public BatchRequests EndBatch() => _parent;
}

public sealed record BatchProgressInfo(
    string BatchName,
    int Completed,
    int Total,
    int Errors
)
{
    public double PercentComplete => Total > 0 ? (double)Completed / Total * 100 : 0;
    public int Remaining => Total - Completed;
}

public sealed record BatchStatistics(
    int BatchCount,
    int TotalQueuedRequests,
    int ProcessedRequests,
    int ErrorCount,
    Dictionary<string, int> RequestsPerBatch
)
{
    public double SuccessRate => ProcessedRequests > 0 
        ? (double)(ProcessedRequests - ErrorCount) / ProcessedRequests * 100 
        : 0;
}

public readonly struct RequeueDecision
{
    public bool ShouldRequeue { get; }
    public Request? ModifiedRequest { get; }

    private RequeueDecision(bool shouldRequeue, Request? modifiedRequest = null)
    {
        ShouldRequeue = shouldRequeue;
        ModifiedRequest = modifiedRequest;
    }

    public static RequeueDecision NoRequeue => new(false);
    public static RequeueDecision Requeue() => new(true);
    public static RequeueDecision RequeueWith(Request modifiedRequest) => new(true, modifiedRequest);
}
