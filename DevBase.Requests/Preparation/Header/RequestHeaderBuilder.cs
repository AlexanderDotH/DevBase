using DevBase.Requests.Abstract;

namespace DevBase.Requests.Preparation.Header;

public class RequestHeaderBuilder : HttpHeaderBuilder<RequestHeaderBuilder>
{
    protected override Action BuildAction { get; }
}