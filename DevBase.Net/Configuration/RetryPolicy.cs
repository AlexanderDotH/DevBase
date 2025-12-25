using DevBase.Net.Configuration.Enums;

namespace DevBase.Net.Configuration;

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
    /// Gets the backoff strategy to use. Defaults to Exponential.
    /// </summary>
    public EnumBackoffStrategy BackoffStrategy { get; init; } = EnumBackoffStrategy.Exponential;
    
    /// <summary>
    /// Gets the initial delay before the first retry. Defaults to 500ms.
    /// </summary>
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(500);
    
    /// <summary>
    /// Gets the maximum delay between retries. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Gets the multiplier for exponential backoff. Defaults to 2.0.
    /// </summary>
    public double BackoffMultiplier { get; init; } = 2.0;

    /// <summary>
    /// Calculates the delay for a specific attempt number.
    /// </summary>
    /// <param name="attemptNumber">The attempt number (1-based).</param>
    /// <returns>The time to wait before the next attempt.</returns>
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

    /// <summary>
    /// Gets the default retry policy (3 retries, exponential backoff).
    /// </summary>
    public static RetryPolicy Default => new();
    
    /// <summary>
    /// Gets a policy with no retries.
    /// </summary>
    public static RetryPolicy None => new() { MaxRetries = 0 };
    
    /// <summary>
    /// Gets an aggressive retry policy (5 retries, short delays).
    /// </summary>
    public static RetryPolicy Aggressive => new()
    {
        MaxRetries = 5,
        BackoffStrategy = EnumBackoffStrategy.Exponential,
        InitialDelay = TimeSpan.FromMilliseconds(100),
        MaxDelay = TimeSpan.FromSeconds(10)
    };
}
