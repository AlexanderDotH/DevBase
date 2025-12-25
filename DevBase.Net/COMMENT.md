# DevBase.Net Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Net project.

## Table of Contents

- [Abstract](#abstract)
  - [GenericBuilder&lt;T&gt;](#genericbuildert)
  - [HttpHeaderBuilder&lt;T&gt;](#httpheaderbuildert)
  - [HttpBodyBuilder&lt;T&gt;](#httpbodybuildert)
  - [HttpFieldBuilder&lt;T&gt;](#httpfieldbuildert)
  - [BogusHttpHeaderBuilder](#bogushttpheaderbuilder)
  - [HttpKeyValueListBuilder&lt;T, K, V&gt;](#httpkeyvaluelistbuildert-k-v)
  - [RequestContent](#requestcontent)
  - [TypographyRequestContent](#typographyrequestcontent)
- [Batch](#batch)
  - [BatchRequests](#batchrequests)
  - [Batch](#batch)
  - [BatchProgressInfo](#batchprogressinfo)
  - [BatchStatistics](#batchstatistics)
  - [RequeueDecision](#requeuedecision)
  - [ProxiedBatchRequests](#proxiedbatchrequests)
  - [ProxiedBatch](#proxiedbatch)
  - [ProxiedBatchStatistics](#proxiedbatchstatistics)
  - [ProxyFailureContext](#proxyfailurecontext)
  - [Proxy Rotation Strategies](#proxy-rotation-strategies)
- [Cache](#cache)
  - [CachedResponse](#cachedresponse)
  - [ResponseCache](#responsecache)
- [Configuration](#configuration)
  - [Enums](#enums)
  - [Configuration Classes](#configuration-classes)
- [Constants](#constants)
- [Core](#core)
  - [BaseRequest](#baserequest)
  - [Request](#request)
  - [BaseResponse](#baseresponse)
  - [Response](#response)
- [Data](#data)
- [Exceptions](#exceptions)
- [Interfaces](#interfaces)
- [Parsing](#parsing)
- [Proxy](#proxy)
- [Security](#security)
- [Utils](#utils)
- [Validation](#validation)

## Abstract

### GenericBuilder&lt;T&gt;

```csharp
/// <summary>
/// Abstract base class for generic builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class GenericBuilder<T> where T : GenericBuilder<T>
{
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (already built).
    /// </summary>
    public bool Usable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericBuilder{T}"/> class.
    /// </summary>
    protected GenericBuilder()
    
    /// <summary>
    /// Gets the action to perform when building.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the object.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the object has already been built.</exception>
    public T Build()
    
    /// <summary>
    /// Attempts to build the object.
    /// </summary>
    /// <returns>True if the build was successful; otherwise, false (if already built).</returns>
    public bool TryBuild()
}
```

### HttpHeaderBuilder&lt;T&gt;

```csharp
/// <summary>
/// Abstract base class for HTTP header builders.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpHeaderBuilder<T> where T : HttpHeaderBuilder<T>
{
    /// <summary>
    /// Gets the StringBuilder used to construct the header.
    /// </summary>
    protected StringBuilder HeaderStringBuilder { get; private set; }
    
    private bool AlreadyBuilt { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether the builder result is usable (built or has content).
    /// </summary>
    public bool Usable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpHeaderBuilder{T}"/> class.
    /// </summary>
    protected HttpHeaderBuilder()
    
    /// <summary>
    /// Gets the action to perform when building the header.
    /// </summary>
    protected abstract Action BuildAction { get; }

    /// <summary>
    /// Builds the HTTP header.
    /// </summary>
    /// <returns>The builder instance.</returns>
    /// <exception cref="HttpHeaderException">Thrown if the header has already been built.</exception>
    public T Build()
}
```

### HttpBodyBuilder&lt;T&gt;

```csharp
/// <summary>
/// Base class for builders that construct HTTP request bodies.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpBodyBuilder<T> where T : HttpBodyBuilder<T>
{
    /// <summary>
    /// Gets the content type of the body.
    /// </summary>
    public abstract string ContentType { get; }
    
    /// <summary>
    /// Gets the content length of the body.
    /// </summary>
    public abstract long ContentLength { get; }
    
    /// <summary>
    /// Gets whether the body is built.
    /// </summary>
    public abstract bool IsBuilt { get; }

    /// <summary>
    /// Builds the body content.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public abstract T Build()
    
    /// <summary>
    /// Writes the body content to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public abstract Task WriteToAsync(Stream stream, CancellationToken cancellationToken = default)
}
```

### HttpFieldBuilder&lt;T&gt;

```csharp
/// <summary>
/// Base class for builders that construct single HTTP fields.
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
public abstract class HttpFieldBuilder<T> where T : HttpFieldBuilder<T>, new()
{
    /// <summary>
    /// Gets whether the field is built.
    /// </summary>
    public bool IsBuilt { get; protected set; }

    /// <summary>
    /// Builds the field.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public abstract T Build()
}
```

### BogusHttpHeaderBuilder

```csharp
/// <summary>
/// Extended header builder with support for fake data generation.
/// </summary>
public class BogusHttpHeaderBuilder : HttpHeaderBuilder<BogusHttpHeaderBuilder>
{
    // Implementation for generating bogus HTTP headers
}
```

### HttpKeyValueListBuilder&lt;T, K, V&gt;

```csharp
/// <summary>
/// Base for key-value pair based body builders (e.g. form-urlencoded).
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
/// <typeparam name="K">The key type.</typeparam>
/// <typeparam name="V">The value type.</typeparam>
public abstract class HttpKeyValueListBuilder<T, K, V> : HttpBodyBuilder<T> 
    where T : HttpKeyValueListBuilder<T, K, V>, new()
{
    /// <summary>
    /// Adds a key-value pair.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The builder instance.</returns>
    public abstract T Add(K key, V value)
}
```

### RequestContent

```csharp
/// <summary>
/// Abstract base for request content validation.
/// </summary>
public abstract class RequestContent
{
    /// <summary>
    /// Validates the request content.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    public abstract bool Validate(string content)
}
```

### TypographyRequestContent

```csharp
/// <summary>
/// Text-based request content validation with encoding.
/// </summary>
public class TypographyRequestContent : RequestContent
{
    /// <summary>
    /// Gets or sets the encoding to use.
    /// </summary>
    public Encoding Encoding { get; set; }
    
    /// <summary>
    /// Validates the text content.
    /// </summary>
    /// <param name="content">The content to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    public override bool Validate(string content)
}
```

## Batch

### BatchRequests

```csharp
/// <summary>
/// High-performance batch request execution engine.
/// </summary>
public sealed class BatchRequests : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the number of batches.
    /// </summary>
    public int BatchCount { get; }
    
    /// <summary>
    /// Gets the total queue count across all batches.
    /// </summary>
    public int TotalQueueCount { get; }
    
    /// <summary>
    /// Gets the response queue count.
    /// </summary>
    public int ResponseQueueCount { get; }
    
    /// <summary>
    /// Gets the rate limit.
    /// </summary>
    public int RateLimit { get; }
    
    /// <summary>
    /// Gets whether cookies are persisted.
    /// </summary>
    public bool PersistCookies { get; }
    
    /// <summary>
    /// Gets whether referer is persisted.
    /// </summary>
    public bool PersistReferer { get; }
    
    /// <summary>
    /// Gets whether processing is active.
    /// </summary>
    public bool IsProcessing { get; }
    
    /// <summary>
    /// Gets the processed count.
    /// </summary>
    public int ProcessedCount { get; }
    
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public int ErrorCount { get; }
    
    /// <summary>
    /// Gets the batch names.
    /// </summary>
    public IReadOnlyList<string> BatchNames { get; }

    /// <summary>
    /// Initializes a new instance of the BatchRequests class.
    /// </summary>
    public BatchRequests()
    
    /// <summary>
    /// Sets the rate limit.
    /// </summary>
    /// <param name="requestsPerWindow">Requests per window.</param>
    /// <param name="window">Time window.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests WithRateLimit(int requestsPerWindow, TimeSpan? window = null)
    
    /// <summary>
    /// Enables cookie persistence.
    /// </summary>
    /// <param name="persist">Whether to persist.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests WithCookiePersistence(bool persist = true)
    
    /// <summary>
    /// Enables referer persistence.
    /// </summary>
    /// <param name="persist">Whether to persist.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests WithRefererPersistence(bool persist = true)
    
    /// <summary>
    /// Creates a new batch.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>The created batch.</returns>
    public Batch CreateBatch(string name)
    
    /// <summary>
    /// Gets or creates a batch.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>The batch.</returns>
    public Batch GetOrCreateBatch(string name)
    
    /// <summary>
    /// Gets a batch by name.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>The batch, or null if not found.</returns>
    public Batch? GetBatch(string name)
    
    /// <summary>
    /// Removes a batch.
    /// </summary>
    /// <param name="name">Batch name.</param>
    /// <returns>True if removed; otherwise, false.</returns>
    public bool RemoveBatch(string name)
    
    /// <summary>
    /// Clears all batches.
    /// </summary>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests ClearAllBatches()
    
    /// <summary>
    /// Adds a response callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnResponse(Func<Response, Task> callback)
    
    /// <summary>
    /// Adds a response callback.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnResponse(Action<Response> callback)
    
    /// <summary>
    /// Adds an error callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnError(Func<Request, System.Exception, Task> callback)
    
    /// <summary>
    /// Adds an error callback.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnError(Action<Request, System.Exception> callback)
    
    /// <summary>
    /// Adds a progress callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnProgress(Func<BatchProgressInfo, Task> callback)
    
    /// <summary>
    /// Adds a progress callback.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnProgress(Action<BatchProgressInfo> callback)
    
    /// <summary>
    /// Adds a response requeue callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnResponseRequeue(Func<Response, Request, RequeueDecision> callback)
    
    /// <summary>
    /// Adds an error requeue callback.
    /// </summary>
    /// <param name="callback">The callback function.</param>
    /// <returns>The BatchRequests instance.</returns>
    public BatchRequests OnErrorRequeue(Func<Request, System.Exception, RequeueDecision> callback)
    
    /// <summary>
    /// Attempts to dequeue a response.
    /// </summary>
    /// <param name="response">The dequeued response.</param>
    /// <returns>True if dequeued; otherwise, false.</returns>
    public bool TryDequeueResponse(out Response? response)
    
    /// <summary>
    /// Dequeues all responses.
    /// </summary>
    /// <returns>List of responses.</returns>
    public List<Response> DequeueAllResponses()
    
    /// <summary>
    /// Starts processing.
    /// </summary>
    public void StartProcessing()
    
    /// <summary>
    /// Stops processing.
    /// </summary>
    public Task StopProcessingAsync()
    
    /// <summary>
    /// Executes all batches.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of responses.</returns>
    public async Task<List<Response>> ExecuteAllAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Executes a specific batch.
    /// </summary>
    /// <param name="batchName">Batch name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of responses.</returns>
    public async Task<List<Response>> ExecuteBatchAsync(string batchName, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Executes all batches as async enumerable.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of responses.</returns>
    public async IAsyncEnumerable<Response> ExecuteAllAsyncEnumerable(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Resets counters.
    /// </summary>
    public void ResetCounters()
    
    /// <summary>
    /// Gets statistics.
    /// </summary>
    /// <returns>Batch statistics.</returns>
    public BatchStatistics GetStatistics()
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public async ValueTask DisposeAsync()
}
```

### Batch

```csharp
/// <summary>
/// Represents a named batch of requests within a BatchRequests engine.
/// </summary>
public sealed class Batch
{
    /// <summary>
    /// Gets the name of the batch.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets the number of items in the queue.
    /// </summary>
    public int QueueCount { get; }

    /// <summary>
    /// Adds a request to the batch.
    /// </summary>
    /// <param name="request">The request to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(Request request)
    
    /// <summary>
    /// Adds a collection of requests.
    /// </summary>
    /// <param name="requests">The requests to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(IEnumerable<Request> requests)
    
    /// <summary>
    /// Adds a request by URL.
    /// </summary>
    /// <param name="url">The URL to request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(string url)
    
    /// <summary>
    /// Adds a collection of URLs.
    /// </summary>
    /// <param name="urls">The URLs to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(IEnumerable<string> urls)
    
    /// <summary>
    /// Enqueues a request (alias for Add).
    /// </summary>
    public Batch Enqueue(Request request)
    
    /// <summary>
    /// Enqueues a request by URL (alias for Add).
    /// </summary>
    public Batch Enqueue(string url)
    
    /// <summary>
    /// Enqueues a collection of requests (alias for Add).
    /// </summary>
    public Batch Enqueue(IEnumerable<Request> requests)
    
    /// <summary>
    /// Enqueues a collection of URLs (alias for Add).
    /// </summary>
    public Batch Enqueue(IEnumerable<string> urls)
    
    /// <summary>
    /// Enqueues a request with configuration.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="configure">Action to configure the request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Enqueue(string url, Action<Request> configure)
    
    /// <summary>
    /// Enqueues a request from a factory.
    /// </summary>
    /// <param name="requestFactory">The factory function.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Enqueue(Func<Request> requestFactory)
    
    /// <summary>
    /// Attempts to dequeue a request.
    /// </summary>
    /// <param name="request">The dequeued request.</param>
    /// <returns>True if dequeued; otherwise, false.</returns>
    public bool TryDequeue(out Request? request)
    
    /// <summary>
    /// Clears all requests.
    /// </summary>
    public void Clear()
    
    /// <summary>
    /// Returns to the parent BatchRequests.
    /// </summary>
    /// <returns>The parent engine.</returns>
    public BatchRequests EndBatch()
}
```

### BatchProgressInfo

```csharp
/// <summary>
/// Information about batch processing progress.
/// </summary>
public class BatchProgressInfo
{
    /// <summary>
    /// Gets the batch name.
    /// </summary>
    public string BatchName { get; }
    
    /// <summary>
    /// Gets the completed count.
    /// </summary>
    public int Completed { get; }
    
    /// <summary>
    /// Gets the total count.
    /// </summary>
    public int Total { get; }
    
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public int Errors { get; }
    
    /// <summary>
    /// Gets the progress percentage.
    /// </summary>
    public double ProgressPercentage { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="batchName">Batch name.</param>
    /// <param name="completed">Completed count.</param>
    /// <param name="total">Total count.</param>
    /// <param name="errors">Error count.</param>
    public BatchProgressInfo(string batchName, int completed, int total, int errors)
}
```

### BatchStatistics

```csharp
/// <summary>
/// Statistics for batch processing.
/// </summary>
public class BatchStatistics
{
    /// <summary>
    /// Gets the batch count.
    /// </summary>
    public int BatchCount { get; }
    
    /// <summary>
    /// Gets the total queue count.
    /// </summary>
    public int TotalQueueCount { get; }
    
    /// <summary>
    /// Gets the processed count.
    /// </summary>
    public int ProcessedCount { get; }
    
    /// <summary>
    /// Gets the error count.
    /// </summary>
    public int ErrorCount { get; }
    
    /// <summary>
    /// Gets the batch queue counts.
    /// </summary>
    public IReadOnlyDictionary<string, int> BatchQueueCounts { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="batchCount">Batch count.</param>
    /// <param name="totalQueueCount">Total queue count.</param>
    /// <param name="processedCount">Processed count.</param>
    /// <param name="errorCount">Error count.</param>
    /// <param name="batchQueueCounts">Batch queue counts.</param>
    public BatchStatistics(int batchCount, int totalQueueCount, int processedCount, int errorCount, IReadOnlyDictionary<string, int> batchQueueCounts)
}
```

### RequeueDecision

```csharp
/// <summary>
/// Decision for requeuing a request.
/// </summary>
public class RequeueDecision
{
    /// <summary>
    /// Gets whether to requeue.
    /// </summary>
    public bool ShouldRequeue { get; }
    
    /// <summary>
    /// Gets the modified request (if any).
    /// </summary>
    public Request? ModifiedRequest { get; }
    
    /// <summary>
    /// Gets a decision to not requeue.
    /// </summary>
    public static RequeueDecision NoRequeue { get; }
    
    /// <summary>
    /// Gets a decision to requeue.
    /// </summary>
    /// <param name="modifiedRequest">Optional modified request.</param>
    /// <returns>The requeue decision.</returns>
    public static RequeueDecision Requeue(Request? modifiedRequest = null)
}
```

### ProxiedBatchRequests

```csharp
/// <summary>
/// Extension of BatchRequests with built-in proxy support.
/// </summary>
public sealed class ProxiedBatchRequests : BatchRequests
{
    /// <summary>
    /// Gets the proxy rotation strategy.
    /// </summary>
    public IProxyRotationStrategy ProxyRotationStrategy { get; }
    
    /// <summary>
    /// Gets the proxy pool.
    /// </summary>
    public IReadOnlyList<TrackedProxyInfo> ProxyPool { get; }

    /// <summary>
    /// Configures the proxy pool.
    /// </summary>
    /// <param name="proxies">List of proxies.</param>
    /// <returns>The ProxiedBatchRequests instance.</returns>
    public ProxiedBatchRequests WithProxies(IList<TrackedProxyInfo> proxies)
    
    /// <summary>
    /// Sets the proxy rotation strategy.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    /// <returns>The ProxiedBatchRequests instance.</returns>
    public ProxiedBatchRequests WithProxyRotationStrategy(IProxyRotationStrategy strategy)
    
    /// <summary>
    /// Gets proxy statistics.
    /// </summary>
    /// <returns>Proxy statistics.</returns>
    public ProxiedBatchStatistics GetProxyStatistics()
}
```

### ProxiedBatch

```csharp
/// <summary>
/// A batch with proxy support.
/// </summary>
public sealed class ProxiedBatch : Batch
{
    /// <summary>
    /// Gets the assigned proxy.
    /// </summary>
    public TrackedProxyInfo? AssignedProxy { get; }
    
    /// <summary>
    /// Gets the proxy failure count.
    /// </summary>
    public int ProxyFailureCount { get; }
    
    /// <summary>
    /// Marks proxy as failed.
    /// </summary>
    public void MarkProxyAsFailed()
    
    /// <summary>
    /// Resets proxy failure count.
    /// </summary>
    public void ResetProxyFailureCount()
}
```

### ProxiedBatchStatistics

```csharp
/// <summary>
/// Statistics for proxied batch processing.
/// </summary>
public class ProxiedBatchStatistics : BatchStatistics
{
    /// <summary>
    /// Gets the total proxy count.
    /// </summary>
    public int TotalProxyCount { get; }
    
    /// <summary>
    /// Gets the active proxy count.
    /// </summary>
    public int ActiveProxyCount { get; }
    
    /// <summary>
    /// Gets the failed proxy count.
    /// </summary>
    public int FailedProxyCount { get; }
    
    /// <summary>
    /// Gets proxy failure details.
    /// </summary>
    public IReadOnlyDictionary<string, int> ProxyFailureCounts { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="baseStats">Base statistics.</param>
    /// <param name="totalProxyCount">Total proxy count.</param>
    /// <param name="activeProxyCount">Active proxy count.</param>
    /// <param name="failedProxyCount">Failed proxy count.</param>
    /// <param name="proxyFailureCounts">Proxy failure counts.</param>
    public ProxiedBatchStatistics(BatchStatistics baseStats, int totalProxyCount, int activeProxyCount, int failedProxyCount, IReadOnlyDictionary<string, int> proxyFailureCounts)
}
```

### ProxyFailureContext

```csharp
/// <summary>
/// Context for proxy failure.
/// </summary>
public class ProxyFailureContext
{
    /// <summary>
    /// Gets the failed proxy.
    /// </summary>
    public TrackedProxyInfo Proxy { get; }
    
    /// <summary>
    /// Gets the exception.
    /// </summary>
    public System.Exception Exception { get; }
    
    /// <summary>
    /// Gets the failure count.
    /// </summary>
    public int FailureCount { get; }
    
    /// <summary>
    /// Gets the timestamp of failure.
    /// </summary>
    public DateTime FailureTimestamp { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="failureCount">Failure count.</param>
    public ProxyFailureContext(TrackedProxyInfo proxy, System.Exception exception, int failureCount)
}
```

### Proxy Rotation Strategies

```csharp
/// <summary>
/// Interface for proxy rotation strategies.
/// </summary>
public interface IProxyRotationStrategy
{
    /// <summary>
    /// Selects the next proxy.
    /// </summary>
    /// <param name="availableProxies">Available proxies.</param>
    /// <param name="failureContexts">Failure contexts.</param>
    /// <returns>The selected proxy, or null if none available.</returns>
    TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Round-robin proxy rotation strategy.
/// </summary>
public class RoundRobinStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Selects the next proxy in round-robin order.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Random proxy rotation strategy.
/// </summary>
public class RandomStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Selects a random proxy.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Least failures proxy rotation strategy.
/// </summary>
public class LeastFailuresStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Selects the proxy with the least failures.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}

/// <summary>
/// Sticky proxy rotation strategy (keeps using the same proxy until it fails).
/// </summary>
public class StickyStrategy : IProxyRotationStrategy
{
    /// <summary>
    /// Gets or sets the current sticky proxy.
    /// </summary>
    public TrackedProxyInfo? CurrentProxy { get; set; }
    
    /// <summary>
    /// Selects the current proxy if available, otherwise selects a new one.
    /// </summary>
    public TrackedProxyInfo? SelectNextProxy(IReadOnlyList<TrackedProxyInfo> availableProxies, IReadOnlyDictionary<string, ProxyFailureContext> failureContexts)
}
```

## Cache

### CachedResponse

```csharp
/// <summary>
/// Represents a cached HTTP response.
/// </summary>
public class CachedResponse
{
    /// <summary>
    /// Gets the cached response data.
    /// </summary>
    public Response Response { get; }
    
    /// <summary>
    /// Gets the cache timestamp.
    /// </summary>
    public DateTime CachedAt { get; }
    
    /// <summary>
    /// Gets the expiration time.
    /// </summary>
    public DateTime? ExpiresAt { get; }
    
    /// <summary>
    /// Gets whether the response is expired.
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="expiresAt">Expiration time.</param>
    public CachedResponse(Response response, DateTime? expiresAt = null)
}
```

### ResponseCache

```csharp
/// <summary>
/// Integrated caching system using FusionCache.
/// </summary>
public class ResponseCache : IDisposable
{
    private readonly FusionCache _cache;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="options">Cache options.</param>
    public ResponseCache(FusionCacheOptions? options = null)
    
    /// <summary>
    /// Gets a cached response.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached response, or null if not found.</returns>
    public async Task<CachedResponse?> GetAsync(Request request, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Sets a response in cache.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    /// <param name="expiration">Expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetAsync(Request request, Response response, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Removes a cached response.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RemoveAsync(Request request, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Clear()
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
}
```

## Configuration

### Enums

#### EnumBackoffStrategy

```csharp
/// <summary>
/// Strategy for retry backoff.
/// </summary>
public enum EnumBackoffStrategy
{
    /// <summary>Fixed delay between retries.</summary>
    Fixed,
    /// <summary>Linear increase in delay.</summary>
    Linear,
    /// <summary>Exponential increase in delay.</summary>
    Exponential
}
```

#### EnumBrowserProfile

```csharp
/// <summary>
/// Browser profile for emulating specific browsers.
/// </summary>
public enum EnumBrowserProfile
{
    /// <summary>No specific profile.</summary>
    None,
    /// <summary>Chrome browser.</summary>
    Chrome,
    /// <summary>Firefox browser.</summary>
    Firefox,
    /// <summary>Edge browser.</summary>
    Edge,
    /// <summary>Safari browser.</summary>
    Safari
}
```

#### EnumHostCheckMethod

```csharp
/// <summary>
/// Method for checking host availability.
/// </summary>
public enum EnumHostCheckMethod
{
    /// <summary>Use ICMP ping.</summary>
    Ping,
    /// <summary>Use TCP connection.</summary>
    TcpConnect
}
```

#### EnumRefererStrategy

```csharp
/// <summary>
/// Strategy for handling referer headers.
/// </summary>
public enum EnumRefererStrategy
{
    /// <summary>No referer.</summary>
    None,
    /// <summary>Use previous URL as referer.</summary>
    PreviousUrl,
    /// <summary>Use domain root as referer.</summary>
    DomainRoot,
    /// <summary>Use custom referer.</summary>
    Custom
}
```

#### EnumRequestLogLevel

```csharp
/// <summary>
/// Log level for request/response logging.
/// </summary>
public enum EnumRequestLogLevel
{
    /// <summary>No logging.</summary>
    None,
    /// <summary>Log only basic info.</summary>
    Basic,
    /// <summary>Log headers.</summary>
    Headers,
    /// <summary>Log full content.</summary>
    Full
}
```

### Configuration Classes

#### RetryPolicy

```csharp
/// <summary>
/// Configuration for request retry policies.
/// </summary>
public sealed class RetryPolicy
{
    /// <summary>
    /// Gets the maximum number of retries. Defaults to 3.
    /// </summary>
    public int MaxRetries { get; init; } = 3;
    
    /// <summary>
    /// Gets the backoff strategy. Defaults to Exponential.
    /// </summary>
    public EnumBackoffStrategy BackoffStrategy { get; init; } = EnumBackoffStrategy.Exponential;
    
    /// <summary>
    /// Gets the initial delay. Defaults to 500ms.
    /// </summary>
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    
    /// <summary>
    /// Gets the maximum delay. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Gets the backoff multiplier. Defaults to 2.0.
    /// </summary>
    public double BackoffMultiplier { get; init; } = 2.0;

    /// <summary>
    /// Calculates delay for a specific attempt.
    /// </summary>
    /// <param name="attemptNumber">Attempt number (1-based).</param>
    /// <returns>Time to wait.</returns>
    public TimeSpan GetDelay(int attemptNumber)
    
    /// <summary>
    /// Gets the default retry policy.
    /// </summary>
    public static RetryPolicy Default { get; }
    
    /// <summary>
    /// Gets a policy with no retries.
    /// </summary>
    public static RetryPolicy None { get; }
    
    /// <summary>
    /// Gets an aggressive retry policy.
    /// </summary>
    public static RetryPolicy Aggressive { get; }
}
```

#### HostCheckConfig

```csharp
/// <summary>
/// Configuration for host availability checks.
/// </summary>
public class HostCheckConfig
{
    /// <summary>
    /// Gets or sets the check method.
    /// </summary>
    public EnumHostCheckMethod Method { get; set; } = EnumHostCheckMethod.Ping;
    
    /// <summary>
    /// Gets or sets the timeout for checks.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    
    /// <summary>
    /// Gets or sets the port for TCP checks.
    /// </summary>
    public int Port { get; set; } = 80;
    
    /// <summary>
    /// Gets or sets whether to enable checks.
    /// </summary>
    public bool Enabled { get; set; } = false;
}
```

#### JsonPathConfig

```csharp
/// <summary>
/// Configuration for JSON path extraction.
/// </summary>
public class JsonPathConfig
{
    /// <summary>
    /// Gets or sets whether to use fast streaming parser.
    /// </summary>
    public bool UseStreamingParser { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the buffer size for streaming.
    /// </summary>
    public int BufferSize { get; set; } = 8192;
    
    /// <summary>
    /// Gets or sets whether to cache compiled paths.
    /// </summary>
    public bool CacheCompiledPaths { get; set; } = true;
}
```

#### LoggingConfig

```csharp
/// <summary>
/// Configuration for request/response logging.
/// </summary>
public class LoggingConfig
{
    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    public EnumRequestLogLevel LogLevel { get; set; } = EnumRequestLogLevel.Basic;
    
    /// <summary>
    /// Gets or sets whether to log request body.
    /// </summary>
    public bool LogRequestBody { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to log response body.
    /// </summary>
    public bool LogResponseBody { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the maximum body size to log.
    /// </summary>
    public int MaxBodySizeToLog { get; set; } = 1024 * 1024; // 1MB
    
    /// <summary>
    /// Gets or sets whether to sanitize headers.
    /// </summary>
    public bool SanitizeHeaders { get; set; } = true;
}
```

#### MultiSelectorConfig

```csharp
/// <summary>
/// Configuration for selecting multiple JSON paths.
/// </summary>
public class MultiSelectorConfig
{
    /// <summary>
    /// Gets the selectors.
    /// </summary>
    public IReadOnlyList<(string name, string path)> Selectors { get; }
    
    /// <summary>
    /// Gets whether to use optimized parsing.
    /// </summary>
    public bool UseOptimizedParsing { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="selectors">The selectors.</param>
    /// <param name="useOptimizedParsing">Whether to use optimized parsing.</param>
    public MultiSelectorConfig(IReadOnlyList<(string name, string path)> selectors, bool useOptimizedParsing = true)
}
```

#### ScrapingBypassConfig

```csharp
/// <summary>
/// Configuration for anti-scraping bypass.
/// </summary>
public class ScrapingBypassConfig
{
    /// <summary>
    /// Gets or sets the referer strategy.
    /// </summary>
    public EnumRefererStrategy RefererStrategy { get; set; } = EnumRefererStrategy.None;
    
    /// <summary>
    /// Gets or sets the custom referer.
    /// </summary>
    public string? CustomReferer { get; set; }
    
    /// <summary>
    /// Gets or sets the browser profile.
    /// </summary>
    public EnumBrowserProfile BrowserProfile { get; set; } = EnumBrowserProfile.None;
    
    /// <summary>
    /// Gets or sets whether to randomize user agent.
    /// </summary>
    public bool RandomizeUserAgent { get; set; } = false;
    
    /// <summary>
    /// Gets or sets additional headers to add.
    /// </summary>
    public Dictionary<string, string> AdditionalHeaders { get; set; } = new();
}
```

## Constants

### AuthConstants

```csharp
/// <summary>
/// Constants for authentication.
/// </summary>
public static class AuthConstants
{
    /// <summary>Bearer authentication scheme.</summary>
    public const string Bearer = "Bearer";
    /// <summary>Basic authentication scheme.</summary>
    public const string Basic = "Basic";
    /// <summary>Digest authentication scheme.</summary>
    public const string Digest = "Digest";
}
```

### EncodingConstants

```csharp
/// <summary>
/// Constants for encoding.
/// </summary>
public static class EncodingConstants
{
    /// <summary>UTF-8 encoding name.</summary>
    public const string Utf8 = "UTF-8";
    /// <summary>ASCII encoding name.</summary>
    public const string Ascii = "ASCII";
    /// <summary>ISO-8859-1 encoding name.</summary>
    public const string Iso88591 = "ISO-8859-1";
}
```

### HeaderConstants

```csharp
/// <summary>
/// Constants for HTTP headers.
/// </summary>
public static class HeaderConstants
{
    /// <summary>Content-Type header.</summary>
    public const string ContentType = "Content-Type";
    /// <summary>Content-Length header.</summary>
    public const string ContentLength = "Content-Length";
    /// <summary>User-Agent header.</summary>
    public const string UserAgent = "User-Agent";
    /// <summary>Authorization header.</summary>
    public const string Authorization = "Authorization";
    /// <summary>Accept header.</summary>
    public const string Accept = "Accept";
    /// <summary>Cookie header.</summary>
    public const string Cookie = "Cookie";
    /// <summary>Set-Cookie header.</summary>
    public const string SetCookie = "Set-Cookie";
    /// <summary>Referer header.</summary>
    public const string Referer = "Referer";
}
```

### HttpConstants

```csharp
/// <summary>
/// Constants for HTTP.
/// </summary>
public static class HttpConstants
{
    /// <summary>HTTP/1.1 version.</summary>
    public const string Http11 = "HTTP/1.1";
    /// <summary>HTTP/2 version.</summary>
    public const string Http2 = "HTTP/2";
    /// <summary>HTTP/3 version.</summary>
    public const string Http3 = "HTTP/3";
}
```

### MimeConstants

```csharp
/// <summary>
/// Constants for MIME types.
/// </summary>
public static class MimeConstants
{
    /// <summary>JSON MIME type.</summary>
    public const string ApplicationJson = "application/json";
    /// <summary>XML MIME type.</summary>
    public const string ApplicationXml = "application/xml";
    /// <summary>Form URL-encoded MIME type.</summary>
    public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";
    /// <summary>Multipart form-data MIME type.</summary>
    public const string MultipartFormData = "multipart/form-data";
    /// <summary>Text HTML MIME type.</summary>
    public const string TextHtml = "text/html";
    /// <summary>Text plain MIME type.</summary>
    public const string TextPlain = "text/plain";
}
```

### PlatformConstants

```csharp
/// <summary>
/// Constants for platforms.
/// </summary>
public static class PlatformConstants
{
    /// <summary>Windows platform.</summary>
    public const string Windows = "Windows";
    /// <summary>Linux platform.</summary>
    public const string Linux = "Linux";
    /// <summary>macOS platform.</summary>
    public const string MacOS = "macOS";
}
```

### ProtocolConstants

```csharp
/// <summary>
/// Constants for protocols.
/// </summary>
public static class ProtocolConstants
{
    /// <summary>HTTP protocol.</summary>
    public const string Http = "http";
    /// <summary>HTTPS protocol.</summary>
    public const string Https = "https";
    /// <summary>WebSocket protocol.</summary>
    public const string Ws = "ws";
    /// <summary>WebSocket Secure protocol.</summary>
    public const string Wss = "wss";
}
```

### UserAgentConstants

```csharp
/// <summary>
/// Constants for user agents.
/// </summary>
public static class UserAgentConstants
{
    /// <summary>Chrome user agent.</summary>
    public const string Chrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
    /// <summary>Firefox user agent.</summary>
    public const string Firefox = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0";
    /// <summary>Edge user agent.</summary>
    public const string Edge = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.59";
}
```

## Core

### BaseRequest

```csharp
/// <summary>
/// Abstract base class for HTTP requests providing core properties and lifecycle management.
/// </summary>
public abstract class BaseRequest : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    public HttpMethod Method { get; }
    
    /// <summary>
    /// Gets the timeout duration.
    /// </summary>
    public TimeSpan Timeout { get; }
    
    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }
    
    /// <summary>
    /// Gets the proxy configuration.
    /// </summary>
    public TrackedProxyInfo? Proxy { get; }
    
    /// <summary>
    /// Gets the retry policy.
    /// </summary>
    public RetryPolicy RetryPolicy { get; }
    
    /// <summary>
    /// Gets whether certificate validation is enabled.
    /// </summary>
    public bool ValidateCertificates { get; }
    
    /// <summary>
    /// Gets whether redirects are followed.
    /// </summary>
    public bool FollowRedirects { get; }
    
    /// <summary>
    /// Gets the maximum redirects.
    /// </summary>
    public int MaxRedirects { get; }
    
    /// <summary>
    /// Gets whether the request is built.
    /// </summary>
    public bool IsBuilt { get; }
    
    /// <summary>
    /// Gets the request interceptors.
    /// </summary>
    public IReadOnlyList<IRequestInterceptor> RequestInterceptors { get; }
    
    /// <summary>
    /// Gets the response interceptors.
    /// </summary>
    public IReadOnlyList<IResponseInterceptor> ResponseInterceptors { get; }

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public abstract ReadOnlySpan<char> Uri { get; }
    
    /// <summary>
    /// Gets the request body.
    /// </summary>
    public abstract ReadOnlySpan<byte> Body { get; }

    /// <summary>
    /// Builds the request.
    /// </summary>
    /// <returns>The built request.</returns>
    public abstract BaseRequest Build()
    
    /// <summary>
    /// Sends the request asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response.</returns>
    public abstract Task<Response> SendAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public virtual void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public virtual ValueTask DisposeAsync()
}
```

### Request

```csharp
/// <summary>
/// HTTP request class with full request building and execution capabilities.
/// Split across partial classes: Request.cs (core), RequestConfiguration.cs (fluent API), 
/// RequestHttp.cs (HTTP execution), RequestContent.cs (content handling), RequestBuilder.cs (file uploads).
/// </summary>
public partial class Request : BaseRequest
{
    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public override ReadOnlySpan<char> Uri { get; }
    
    /// <summary>
    /// Gets the request body.
    /// </summary>
    public override ReadOnlySpan<byte> Body { get; }
    
    /// <summary>
    /// Gets the request URI as Uri object.
    /// </summary>
    public Uri? GetUri()
    
    /// <summary>
    /// Gets the scraping bypass configuration.
    /// </summary>
    public ScrapingBypassConfig? ScrapingBypass { get; }
    
    /// <summary>
    /// Gets the JSON path configuration.
    /// </summary>
    public JsonPathConfig? JsonPathConfig { get; }
    
    /// <summary>
    /// Gets the host check configuration.
    /// </summary>
    public HostCheckConfig? HostCheckConfig { get; }
    
    /// <summary>
    /// Gets the logging configuration.
    /// </summary>
    public LoggingConfig? LoggingConfig { get; }
    
    /// <summary>
    /// Gets whether header validation is enabled.
    /// </summary>
    public bool HeaderValidationEnabled { get; }
    
    /// <summary>
    /// Gets the header builder.
    /// </summary>
    public RequestHeaderBuilder? HeaderBuilder { get; }
    
    /// <summary>
    /// Gets the request interceptors.
    /// </summary>
    public new IReadOnlyList<IRequestInterceptor> RequestInterceptors { get; }
    
    /// <summary>
    /// Gets the response interceptors.
    /// </summary>
    public new IReadOnlyList<IResponseInterceptor> ResponseInterceptors { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public Request()
    
    /// <summary>
    /// Initializes with URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    public Request(ReadOnlyMemory<char> url)
    
    /// <summary>
    /// Initializes with URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    public Request(string url)
    
    /// <summary>
    /// Initializes with URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    public Request(Uri uri)
    
    /// <summary>
    /// Initializes with URL and method.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="method">The HTTP method.</param>
    public Request(string url, HttpMethod method)
    
    /// <summary>
    /// Initializes with URI and method.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="method">The HTTP method.</param>
    public Request(Uri uri, HttpMethod method)
    
    /// <summary>
    /// Builds the request.
    /// </summary>
    /// <returns>The built request.</returns>
    public override BaseRequest Build()
    
    /// <summary>
    /// Sends the request asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response.</returns>
    public override async Task<Response> SendAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public override void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public override ValueTask DisposeAsync()
}
```

### BaseResponse

```csharp
/// <summary>
/// Abstract base class for HTTP responses providing core properties and content access.
/// </summary>
public abstract class BaseResponse : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }
    
    /// <summary>
    /// Gets whether the response indicates success.
    /// </summary>
    public bool IsSuccessStatusCode { get; }
    
    /// <summary>
    /// Gets the response headers.
    /// </summary>
    public HttpResponseHeaders Headers { get; }
    
    /// <summary>
    /// Gets the content headers.
    /// </summary>
    public HttpContentHeaders? ContentHeaders { get; }
    
    /// <summary>
    /// Gets the content type.
    /// </summary>
    public string? ContentType { get; }
    
    /// <summary>
    /// Gets the content length.
    /// </summary>
    public long? ContentLength { get; }
    
    /// <summary>
    /// Gets the HTTP version.
    /// </summary>
    public Version HttpVersion { get; }
    
    /// <summary>
    /// Gets the reason phrase.
    /// </summary>
    public string? ReasonPhrase { get; }
    
    /// <summary>
    /// Gets whether this is a redirect response.
    /// </summary>
    public bool IsRedirect { get; }
    
    /// <summary>
    /// Gets whether this is a client error (4xx).
    /// </summary>
    public bool IsClientError { get; }
    
    /// <summary>
    /// Gets whether this is a server error (5xx).
    /// </summary>
    public bool IsServerError { get; }
    
    /// <summary>
    /// Gets whether this response indicates rate limiting.
    /// </summary>
    public bool IsRateLimited { get; }

    /// <summary>
    /// Gets the response content as bytes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The content bytes.</returns>
    public virtual async Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Gets the response content as string.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The content string.</returns>
    public virtual async Task<string> GetStringAsync(Encoding? encoding = null, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Gets the response content stream.
    /// </summary>
    /// <returns>The content stream.</returns>
    public virtual Stream GetStream()
    
    /// <summary>
    /// Gets cookies from the response.
    /// </summary>
    /// <returns>The cookie collection.</returns>
    public virtual CookieCollection GetCookies()
    
    /// <summary>
    /// Gets a header value by name.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <returns>The header value.</returns>
    public virtual string? GetHeader(string name)
    
    /// <summary>
    /// Throws if the response does not indicate success.
    /// </summary>
    public virtual void EnsureSuccessStatusCode()
    
    /// <summary>
    /// Disposes resources.
    /// </summary>
    public virtual void Dispose()
    
    /// <summary>
    /// Disposes resources asynchronously.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
}
```

### Response

```csharp
/// <summary>
/// HTTP response class with parsing and streaming capabilities.
/// </summary>
public sealed class Response : BaseResponse
{
    /// <summary>
    /// Gets the request metrics.
    /// </summary>
    public RequestMetrics Metrics { get; }
    
    /// <summary>
    /// Gets whether this response was served from cache.
    /// </summary>
    public bool FromCache { get; }
    
    /// <summary>
    /// Gets the original request URI.
    /// </summary>
    public Uri? RequestUri { get; }

    /// <summary>
    /// Gets the response as specified type.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed response.</returns>
    public async Task<T> GetAsync<T>(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON response.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="useSystemTextJson">Whether to use System.Text.Json.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed object.</returns>
    public async Task<T> ParseJsonAsync<T>(bool useSystemTextJson = true, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON document.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The JsonDocument.</returns>
    public async Task<JsonDocument> ParseJsonDocumentAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses XML response.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The XDocument.</returns>
    public async Task<XDocument> ParseXmlAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses HTML response.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The IDocument.</returns>
    public async Task<IDocument> ParseHtmlAsync(CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON path.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="path">The JSON path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed value.</returns>
    public async Task<T> ParseJsonPathAsync<T>(string path, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses JSON path list.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="path">The JSON path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed list.</returns>
    public async Task<List<T>> ParseJsonPathListAsync<T>(string path, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses multiple JSON paths.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The multi-selector result.</returns>
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(MultiSelectorConfig config, CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Parses multiple JSON paths.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="selectors">The selectors.</param>
    /// <returns>The multi-selector result.</returns>
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsAsync(CancellationToken cancellationToken = default, params (string name, string path)[] selectors)
    
    /// <summary>
    /// Parses multiple JSON paths optimized.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="selectors">The selectors.</param>
    /// <returns>The multi-selector result.</returns>
    public async Task<MultiSelectorResult> ParseMultipleJsonPathsOptimizedAsync(CancellationToken cancellationToken = default, params (string name, string path)[] selectors)
    
    /// <summary>
    /// Streams response lines.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of lines.</returns>
    public async IAsyncEnumerable<string> StreamLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Streams response chunks.
    /// </summary>
    /// <param name="chunkSize">Chunk size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of chunks.</returns>
    public async IAsyncEnumerable<byte[]> StreamChunksAsync(int chunkSize = 4096, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    
    /// <summary>
    /// Gets header values.
    /// </summary>
    /// <param name="name">Header name.</param>
    /// <returns>The header values.</returns>
    public IEnumerable<string> GetHeaderValues(string name)
    
    /// <summary>
    /// Parses bearer token.
    /// </summary>
    /// <returns>The authentication token.</returns>
    public AuthenticationToken? ParseBearerToken()
    
    /// <summary>
    /// Parses and verifies bearer token.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>The authentication token.</returns>
    public AuthenticationToken? ParseAndVerifyBearerToken(string secret)
    
    /// <summary>
    /// Validates content length.
    /// </summary>
    /// <returns>The validation result.</returns>
    public ValidationResult ValidateContentLength()
}
```

## Data

The Data namespace contains various data structures for HTTP requests and responses:

- **Body**: Classes for different body types (JsonBody, FormBody, MultipartBody, etc.)
- **Header**: Classes for HTTP headers (RequestHeaderBuilder, ResponseHeaders, etc.)
- **Query**: Classes for query string handling
- **Cookie**: Classes for cookie handling
- **Mime**: Classes for MIME type handling

## Exceptions

The Exceptions namespace contains custom exceptions:

- **HttpHeaderException**: Thrown for HTTP header errors
- **RequestException**: Base class for request errors
- **ResponseException**: Base class for response errors
- **ProxyException**: Thrown for proxy-related errors
- **ValidationException**: Thrown for validation errors

## Interfaces

The Interfaces namespace defines contracts for:

- **IRequestInterceptor**: Interface for request interceptors
- **IResponseInterceptor**: Interface for response interceptors
- **IHttpClient**: Interface for HTTP clients
- **IProxyProvider**: Interface for proxy providers

## Parsing

The Parsing namespace provides parsers for:

- **JsonPathParser**: JSON path extraction
- **StreamingJsonPathParser**: Fast streaming JSON path parser
- **MultiSelectorParser**: Multiple JSON path selector
- **HtmlParser**: HTML parsing utilities
- **XmlParser**: XML parsing utilities

## Proxy

The Proxy namespace contains:

- **TrackedProxyInfo**: Information about a proxy with tracking
- **ProxyValidator**: Proxy validation utilities
- **ProxyPool**: Pool of proxies
- **ProxyRotator**: Proxy rotation logic

## Security

The Security namespace provides:

- **Token**: JWT token handling
- **AuthenticationToken**: Authentication token structure
- **JwtValidator**: JWT validation utilities
- **CertificateValidator**: Certificate validation

## Utils

The Utils namespace contains utility classes:

- **BogusUtils**: Fake data generation
- **JsonUtils**: JSON manipulation helpers
- **ContentDispositionUtils**: Content-Disposition parsing
- **UriUtils**: URI manipulation utilities
- **StringBuilderPool**: Pool for StringBuilder instances

## Validation

The Validation namespace provides:

- **HeaderValidator**: HTTP header validation
- **RequestValidator**: Request validation
- **ResponseValidator**: Response validation
- **ValidationResult**: Result of validation
