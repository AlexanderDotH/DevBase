using System.Net;
using DevBase.Requests.Abstract;

namespace DevBase.Requests.Preparation.Header.Body;

public class RequestRawBodyBuilder : HttpBodyBuilder<RequestRawBodyBuilder>
{
    public RequestRawBodyBuilder()
    {
        
    }
    
    protected override Action BuildAction { get; }
}