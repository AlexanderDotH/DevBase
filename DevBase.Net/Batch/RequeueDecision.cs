using DevBase.Net.Core;

namespace DevBase.Net.Batch;

public readonly struct RequeueDecision
{
    public bool ShouldRequeue { get; }
    public Request? ModifiedRequest { get; }

    private RequeueDecision(bool shouldRequeue, Request? modifiedRequest = null)
    {
        ShouldRequeue = shouldRequeue;
        ModifiedRequest = modifiedRequest;
    }

    public static RequeueDecision NoRequeue => new(false);
    public static RequeueDecision Requeue() => new(true);
    public static RequeueDecision RequeueWith(Request modifiedRequest) => new(true, modifiedRequest);
}
