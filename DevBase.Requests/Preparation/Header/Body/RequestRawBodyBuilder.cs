using DevBase.Requests.Abstract;

namespace DevBase.Requests.Preparation.Header.Body;

public class RequestRawBodyBuilder : HttpBodyBuilder<RequestRawBodyBuilder>
{
    protected override Action BuildAction { get; }
    // r.Body["",""]
}