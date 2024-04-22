using DevBase.Requests.Data.Parameters;

namespace DevBase.Requests.Data;

public class RequestData
{
    private ParameterBuilder _parameterBuilder;

    public RequestData()
    {
        this._parameterBuilder = new ParameterBuilder();
    }
}