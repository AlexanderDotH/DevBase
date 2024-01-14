using DevBase.Requests.Preparation.Parameters;

namespace DevBase.Requests.Preparation;

public class RequestData
{
    private ParameterBuilder _parameterBuilder;

    public RequestData()
    {
        this._parameterBuilder = new ParameterBuilder();
    }
}