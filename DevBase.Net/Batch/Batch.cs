using System.Collections.Concurrent;
using DevBase.Net.Core;

namespace DevBase.Net.Batch;

/// <summary>
/// Represents a named batch of requests within a <see cref="BatchRequests"/> engine.
/// </summary>
public sealed class Batch
{
    private readonly ConcurrentQueue<Request> _queue = new();
    private readonly BatchRequests _parent;

    /// <summary>
    /// Gets the name of the batch.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets the number of items currently in the queue.
    /// </summary>
    public int QueueCount => _queue.Count;

    internal Batch(string name, BatchRequests parent)
    {
        Name = name;
        _parent = parent;
    }

    /// <summary>
    /// Adds a request to the batch.
    /// </summary>
    /// <param name="request">The request to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(Request request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _queue.Enqueue(request);
        return this;
    }

    /// <summary>
    /// Adds a collection of requests to the batch.
    /// </summary>
    /// <param name="requests">The requests to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(IEnumerable<Request> requests)
    {
        foreach (Request request in requests)
            Add(request);
        return this;
    }

    /// <summary>
    /// Adds a request by URL to the batch.
    /// </summary>
    /// <param name="url">The URL to request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(string url)
    {
        return Add(new Request(url));
    }

    /// <summary>
    /// Adds a collection of URLs to the batch.
    /// </summary>
    /// <param name="urls">The URLs to add.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Add(IEnumerable<string> urls)
    {
        foreach (string url in urls)
            Add(url);
        return this;
    }

    /// <summary>
    /// Enqueues a request (alias for Add).
    /// </summary>
    public Batch Enqueue(Request request) => Add(request);
    
    /// <summary>
    /// Enqueues a request by URL (alias for Add).
    /// </summary>
    public Batch Enqueue(string url) => Add(url);
    
    /// <summary>
    /// Enqueues a collection of requests (alias for Add).
    /// </summary>
    public Batch Enqueue(IEnumerable<Request> requests) => Add(requests);
    
    /// <summary>
    /// Enqueues a collection of URLs (alias for Add).
    /// </summary>
    public Batch Enqueue(IEnumerable<string> urls) => Add(urls);

    /// <summary>
    /// Enqueues a request created from a URL and configured via an action.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="configure">Action to configure the request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Enqueue(string url, Action<Request> configure)
    {
        Request request = new Request(url);
        configure(request);
        return Add(request);
    }

    /// <summary>
    /// Enqueues a request created by a factory function.
    /// </summary>
    /// <param name="requestFactory">The function to create the request.</param>
    /// <returns>The current batch instance.</returns>
    public Batch Enqueue(Func<Request> requestFactory)
    {
        return Add(requestFactory());
    }

    /// <summary>
    /// Attempts to dequeue a request from the batch.
    /// </summary>
    /// <param name="request">The dequeued request, if successful.</param>
    /// <returns>True if a request was dequeued; otherwise, false.</returns>
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

    /// <summary>
    /// Clears all requests from the batch queue.
    /// </summary>
    public void Clear()
    {
        while (_queue.TryDequeue(out _)) { }
    }

    /// <summary>
    /// Returns to the parent <see cref="BatchRequests"/> instance.
    /// </summary>
    /// <returns>The parent engine.</returns>
    public BatchRequests EndBatch() => _parent;
}
