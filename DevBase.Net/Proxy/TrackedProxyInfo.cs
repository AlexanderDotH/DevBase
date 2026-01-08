using System.Net;

namespace DevBase.Net.Proxy;

public class TrackedProxyInfo
{
    private readonly object _lock = new();
    
    public ProxyInfo Proxy { get; }
    public int FailureCount { get; protected set; }
    public DateTime? LastFailure { get; protected set; }
    public bool IsTimedOut { get; protected set; }
    public DateTime? TimeoutUntil { get; protected set; }
    public int TotalTimeouts { get; protected set; }
    public int MaxFailures { get; }
    public TimeSpan TimeoutDuration { get; }
    
    public TimeSpan? RemainingTimeout
    {
        get
        {
            if (!IsTimedOut || TimeoutUntil == null)
                return null;
                
            TimeSpan remaining = TimeoutUntil.Value - DateTime.UtcNow;
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

    public virtual bool ReportFailure()
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

    public virtual void ReportSuccess()
    {
        lock (_lock)
        {
            FailureCount = 0;
            LastFailure = null;
        }
    }

    public virtual bool IsAvailable()
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

    public virtual void ResetTimeout()
    {
        lock (_lock)
        {
            IsTimedOut = false;
            TimeoutUntil = null;
            FailureCount = 0;
        }
    }

    public virtual IWebProxy ToWebProxy() => Proxy.ToWebProxy();

    public override string ToString() => 
        $"{Key} [Failures: {FailureCount}/{MaxFailures}, TimedOut: {IsTimedOut}]";
}
