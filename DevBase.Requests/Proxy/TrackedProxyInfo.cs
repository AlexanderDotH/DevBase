using System.Net;
using DevBase.Requests.Proxy.Enums;

namespace DevBase.Requests.Proxy;

public sealed class TrackedProxyInfo
{
    private readonly object _lock = new();
    
    public ProxyInfo Proxy { get; }
    public int FailureCount { get; private set; }
    public DateTime? LastFailure { get; private set; }
    public bool IsTimedOut { get; private set; }
    public DateTime? TimeoutUntil { get; private set; }
    public int TotalTimeouts { get; private set; }
    public int MaxFailures { get; }
    public TimeSpan TimeoutDuration { get; }
    
    public TimeSpan? RemainingTimeout
    {
        get
        {
            if (!IsTimedOut || TimeoutUntil == null)
                return null;
                
            var remaining = TimeoutUntil.Value - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : null;
        }
    }
    
    public string Key => Proxy.Key;

    public TrackedProxyInfo(ProxyInfo proxy, int maxFailures = 3, TimeSpan? timeoutDuration = null)
    {
        ArgumentNullException.ThrowIfNull(proxy);
        
        Proxy = proxy;
        MaxFailures = maxFailures;
        TimeoutDuration = timeoutDuration ?? TimeSpan.FromMinutes(10);
        FailureCount = 0;
        TotalTimeouts = 0;
        IsTimedOut = false;
    }

    public bool ReportFailure()
    {
        lock (_lock)
        {
            FailureCount++;
            LastFailure = DateTime.UtcNow;
            
            if (FailureCount >= MaxFailures)
            {
                IsTimedOut = true;
                TimeoutUntil = DateTime.UtcNow.Add(TimeoutDuration);
                TotalTimeouts++;
                return true;
            }
            
            return false;
        }
    }

    public void ReportSuccess()
    {
        lock (_lock)
        {
            FailureCount = 0;
            LastFailure = null;
        }
    }

    public bool IsAvailable()
    {
        lock (_lock)
        {
            if (!IsTimedOut)
                return true;
                
            if (TimeoutUntil.HasValue && DateTime.UtcNow >= TimeoutUntil.Value)
            {
                ResetTimeout();
                return true;
            }
            
            return false;
        }
    }

    public void ResetTimeout()
    {
        lock (_lock)
        {
            IsTimedOut = false;
            TimeoutUntil = null;
            FailureCount = 0;
        }
    }

    public IWebProxy ToWebProxy()
    {
        var webProxy = new WebProxy(Proxy.Host, Proxy.Port);
        
        if (Proxy.Credentials != null)
            webProxy.Credentials = Proxy.Credentials;
            
        return webProxy;
    }

    public override string ToString() => 
        $"{Key} [Failures: {FailureCount}/{MaxFailures}, TimedOut: {IsTimedOut}]";
}
