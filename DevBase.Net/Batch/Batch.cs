using System.Collections.Concurrent;
using DevBase.Net.Core;

namespace DevBase.Net.Batch;

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
