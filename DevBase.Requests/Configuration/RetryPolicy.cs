namespace DevBase.Requests.Configuration;

public enum BackoffStrategy
{
    Fixed,
    Linear,
    Exponential
}

public sealed class RetryPolicy
{
    public int MaxRetries { get; init; } = 3;
    public bool RetryOnProxyError { get; init; } = true;
    public bool RetryOnTimeout { get; init; } = true;
    public bool RetryOnNetworkError { get; init; } = true;
    public BackoffStrategy BackoffStrategy { get; init; } = BackoffStrategy.Exponential;
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);
    public double BackoffMultiplier { get; init; } = 2.0;

    public TimeSpan GetDelay(int attemptNumber)
    {
        if (attemptNumber <= 0)
            return TimeSpan.Zero;

        var delay = BackoffStrategy switch
        {
            BackoffStrategy.Fixed => InitialDelay,
            BackoffStrategy.Linear => TimeSpan.FromTicks(InitialDelay.Ticks * attemptNumber),
            BackoffStrategy.Exponential => TimeSpan.FromTicks(
                (long)(InitialDelay.Ticks * Math.Pow(BackoffMultiplier, attemptNumber - 1))),
            _ => InitialDelay
        };

        return delay > MaxDelay ? MaxDelay : delay;
    }

    public static RetryPolicy Default => new();
    
    public static RetryPolicy None => new() { MaxRetries = 0 };
    
    public static RetryPolicy Aggressive => new()
    {
        MaxRetries = 5,
        BackoffStrategy = BackoffStrategy.Exponential,
        InitialDelay = TimeSpan.FromMilliseconds(100),
        MaxDelay = TimeSpan.FromSeconds(10)
    };
}
