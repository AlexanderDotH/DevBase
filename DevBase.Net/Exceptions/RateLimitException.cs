namespace DevBase.Net.Exceptions;

public class RateLimitException : System.Exception
{
    public TimeSpan? RetryAfter { get; }
    public DateTime? ResetAt { get; }
    public Uri? RequestUri { get; }

    public RateLimitException(string message, TimeSpan? retryAfter = null, DateTime? resetAt = null, Uri? requestUri = null)
        : base(message)
    {
        RetryAfter = retryAfter;
        ResetAt = resetAt;
        RequestUri = requestUri;
    }

    public static RateLimitException FromRetryAfter(int seconds, Uri? requestUri = null) =>
        new($"Rate limited. Retry after {seconds} seconds.",
            TimeSpan.FromSeconds(seconds),
            DateTime.UtcNow.AddSeconds(seconds),
            requestUri);

    public static RateLimitException FromResetTime(DateTime resetAt, Uri? requestUri = null) =>
        new($"Rate limited. Reset at {resetAt:u}",
            resetAt - DateTime.UtcNow,
            resetAt,
            requestUri);
}
