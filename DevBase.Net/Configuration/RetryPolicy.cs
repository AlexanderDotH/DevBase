using DevBase.Net.Configuration.Enums;

namespace DevBase.Net.Configuration;

public sealed class RetryPolicy
{
    public int MaxRetries { get; init; } = 3;
    public EnumBackoffStrategy BackoffStrategy { get; init; } = EnumBackoffStrategy.Exponential;
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);
    public double BackoffMultiplier { get; init; } = 2.0;

    public TimeSpan GetDelay(int attemptNumber)
    {
        if (attemptNumber <= 0)
            return TimeSpan.Zero;

        TimeSpan delay = this.BackoffStrategy switch
        {
            EnumBackoffStrategy.Fixed => this.InitialDelay,
            EnumBackoffStrategy.Linear => TimeSpan.FromTicks(this.InitialDelay.Ticks * attemptNumber),
            EnumBackoffStrategy.Exponential => TimeSpan.FromTicks(
                (long)(this.InitialDelay.Ticks * Math.Pow(this.BackoffMultiplier, attemptNumber - 1))),
            _ => this.InitialDelay
        };

        return delay > this.MaxDelay ? this.MaxDelay : delay;
    }

    public static RetryPolicy Default => new();
    
    public static RetryPolicy None => new() { MaxRetries = 0 };
    
    public static RetryPolicy Aggressive => new()
    {
        MaxRetries = 5,
        BackoffStrategy = EnumBackoffStrategy.Exponential,
        InitialDelay = TimeSpan.FromMilliseconds(100),
        MaxDelay = TimeSpan.FromSeconds(10)
    };
}
