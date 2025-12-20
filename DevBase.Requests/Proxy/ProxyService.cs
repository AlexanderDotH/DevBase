namespace DevBase.Requests.Proxy;

public sealed class ProxyService : IDisposable
{
    private readonly List<TrackedProxyInfo> _trackedProxies;
    private int _currentIndex;
    private readonly object _lock = new();

    public int ValidProxyCount => _trackedProxies.Count;
    public int TotalProxiesProvided { get; }
    public bool HasProxies => _trackedProxies.Count > 0;
    public int TimedOutProxyCount => _trackedProxies.Count(p => !p.IsAvailable());
    public int ActiveProxyCount => _trackedProxies.Count(p => p.IsAvailable());

    public ProxyService(
        List<ProxyInfo> validProxies,
        int totalProvided,
        int maxFailures = 3,
        TimeSpan? timeoutDuration = null)
    {
        TotalProxiesProvided = totalProvided;
        
        _trackedProxies = (validProxies ?? [])
            .Select(p => new TrackedProxyInfo(p, maxFailures, timeoutDuration))
            .ToList();
        
        _currentIndex = 0;
    }

    public ProxyService(List<ProxyInfo> validProxies, int maxFailures = 3, TimeSpan? timeoutDuration = null)
        : this(validProxies, validProxies?.Count ?? 0, maxFailures, timeoutDuration)
    {
    }

    public TrackedProxyInfo? GetNextProxy()
    {
        lock (_lock)
        {
            if (!HasProxies) 
                return null;
            
            for (int i = 0; i < _trackedProxies.Count; i++)
            {
                var proxy = _trackedProxies[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _trackedProxies.Count;
                
                if (proxy.IsAvailable())
                    return proxy;
            }
            
            return _trackedProxies[Random.Shared.Next(_trackedProxies.Count)];
        }
    }

    public TrackedProxyInfo? GetRandomProxy()
    {
        lock (_lock)
        {
            if (!HasProxies) 
                return null;
            
            var availableProxies = _trackedProxies.Where(p => p.IsAvailable()).ToList();
            
            if (availableProxies.Count > 0)
                return availableProxies[Random.Shared.Next(availableProxies.Count)];
            
            return _trackedProxies[Random.Shared.Next(_trackedProxies.Count)];
        }
    }

    public ProxyTimeoutStats GetTimeoutStats()
    {
        lock (_lock)
        {
            var active = 0;
            var timedOut = 0;
            var totalTimeouts = 0;
            
            foreach (var proxy in _trackedProxies)
            {
                if (proxy.IsAvailable())
                    active++;
                else
                    timedOut++;
                    
                totalTimeouts += proxy.TotalTimeouts;
            }
            
            return new ProxyTimeoutStats
            {
                TotalProxies = _trackedProxies.Count,
                ActiveProxies = active,
                TimedOutProxies = timedOut,
                TotalTimeoutEvents = totalTimeouts
            };
        }
    }

    public List<TrackedProxyInfo> GetTimedOutProxies()
    {
        lock (_lock)
        {
            return _trackedProxies
                .Where(p => !p.IsAvailable())
                .OrderByDescending(p => p.TimeoutUntil)
                .ToList();
        }
    }

    public List<TrackedProxyInfo> GetAllTrackedProxies()
    {
        lock (_lock)
        {
            return [.. _trackedProxies];
        }
    }

    public List<ProxyInfo> GetAllProxies()
    {
        lock (_lock)
        {
            return _trackedProxies.Select(p => p.Proxy).ToList();
        }
    }

    public void ResetAllTimeouts()
    {
        lock (_lock)
        {
            foreach (var proxy in _trackedProxies)
                proxy.ResetTimeout();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _trackedProxies.Clear();
        }
    }
}
