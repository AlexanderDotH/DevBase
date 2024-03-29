﻿using System.Text;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Extensions;

namespace DevBase.Requests.Preparation.Parameters;

public class ParameterBuilder : HttpHeaderBuilder<ParameterBuilder>
{
    private char[] _parameters;
    
    public ParameterBuilder()
    {
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
        this.HeaderStringBuilder.Append(this.HeaderStringBuilder.Length == 0 ? '?' : '&');
        
        this.HeaderStringBuilder.Append(key);
        this.HeaderStringBuilder.Append('=');
        this.HeaderStringBuilder.Append(value);
    }

    protected override Action BuildAction => () =>
    {
        this.HeaderStringBuilder.ToSpan(ref this._parameters);
    };
    
    public Span<char> Parameters => this._parameters;
}