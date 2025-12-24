using System.Collections.Concurrent;
using DevBase.Net.Core;

namespace DevBase.Net.Batch.Proxied;

public sealed class ProxiedBatch
{
    private readonly ConcurrentQueue<Request> _queue = new();
    private readonly ProxiedBatchRequests _parent;

    public string Name { get; }
    public int QueueCount => _queue.Count;

    internal ProxiedBatch(string name, ProxiedBatchRequests parent)
    {
        Name = name;
        _parent = parent;
    }

    public ProxiedBatch Add(Request request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _queue.Enqueue(request);
        return this;
    }

    public ProxiedBatch Add(IEnumerable<Request> requests)
    {
        foreach (Request request in requests)
            Add(request);
        return this;
    }

    public ProxiedBatch Add(string url)
    {
        return Add(new Request(url));
    }

    public ProxiedBatch Add(IEnumerable<string> urls)
    {
        foreach (string url in urls)
            Add(url);
        return this;
    }

    public ProxiedBatch Enqueue(Request request) => Add(request);
    public ProxiedBatch Enqueue(string url) => Add(url);
    public ProxiedBatch Enqueue(IEnumerable<Request> requests) => Add(requests);
    public ProxiedBatch Enqueue(IEnumerable<string> urls) => Add(urls);

    public ProxiedBatch Enqueue(string url, Action<Request> configure)
    {
        Request request = new Request(url);
        configure(request);
        return Add(request);
    }

    public ProxiedBatch Enqueue(Func<Request> requestFactory)
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

    public ProxiedBatchRequests EndBatch() => _parent;
}
