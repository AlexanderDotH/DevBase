using DevBase.Net.Core;

namespace DevBase.Net.Batch;

/// <summary>
/// Represents a decision on whether to requeue a request after processing (e.g., on failure or specific response).
/// </summary>
public readonly struct RequeueDecision
{
    /// <summary>
    /// Gets a value indicating whether the request should be requeued.
    /// </summary>
    public bool ShouldRequeue { get; }
    
    /// <summary>
    /// Gets the modified request to requeue, if any. If null, the original request is used (if ShouldRequeue is true).
    /// </summary>
    public Request? ModifiedRequest { get; }

    private RequeueDecision(bool shouldRequeue, Request? modifiedRequest = null)
    {
        ShouldRequeue = shouldRequeue;
        ModifiedRequest = modifiedRequest;
    }

    /// <summary>
    /// Indicates that the request should not be requeued.
    /// </summary>
    public static RequeueDecision NoRequeue => new(false);
    
    /// <summary>
    /// Indicates that the request should be requeued as is.
    /// </summary>
    public static RequeueDecision Requeue() => new(true);
    
    /// <summary>
    /// Indicates that the request should be requeued with modifications.
    /// </summary>
    /// <param name="modifiedRequest">The modified request to queue.</param>
    public static RequeueDecision RequeueWith(Request modifiedRequest) => new(true, modifiedRequest);
}
