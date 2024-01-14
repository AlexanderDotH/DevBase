using System.Text;
using DevBase.Requests.Extensions;

namespace DevBase.Requests.Preparation.Parameters;

public class ParameterBuilder
{
    private StringBuilder _preGeneratedParameters;
    private char[] _parameters;
    
    public ParameterBuilder()
    {
        this._preGeneratedParameters = new StringBuilder();
        this._parameters = Array.Empty<char>();
    }

    public ParameterBuilder AddParameter(string key, string value)
    {
        Append(key, value);
        return this;
    }
    
    public ParameterBuilder AddParameters(params (string key, string value)[] parameters)
    {
        for (var i = 0; i < parameters.Length; i++)
            AddParameter(parameters[i].key, parameters[i].value);

        return this;
    }

    private void Append(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
    {
        this._preGeneratedParameters.Append(this._preGeneratedParameters.Length == 0 ? '?' : '&');
        
        this._preGeneratedParameters.Append(key);
        this._preGeneratedParameters.Append('=');
        this._preGeneratedParameters.Append(value);
    }

    public ParameterBuilder Build()
    {
        this._preGeneratedParameters.ToSpan(ref this._parameters);
        return this;
    }

    public Span<char> Parameters => this._parameters;
}