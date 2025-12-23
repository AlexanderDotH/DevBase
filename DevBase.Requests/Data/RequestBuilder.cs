using System.Text;
using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Body;
using DevBase.Requests.Data.Header;
using DevBase.Requests.Data.Parameters;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Render;

namespace DevBase.Requests.Data;

public class RequestBuilder : GenericBuilder<RequestBuilder>
{
    private ReadOnlyMemory<byte> RenderedBody { get; set; }
    private ReadOnlyMemory<char> RenderedUri { get; set; }
    
    private ParameterBuilder? ParameterBuilder { get; set; }
    
    public RequestHeaderBuilder? RequestHeaderBuilder { get; private set; }
    
    public RequestBuilder() {}
    
    public RequestBuilder(ReadOnlyMemory<char> requestUri) : this()
    {
        this.RenderedUri = requestUri;
    }

    public RequestBuilder(string requestUri) : this(requestUri.AsMemory()) { }
    public RequestBuilder(Uri requestUri) : this(RequestUriRenderer.RenderUri(requestUri)) { }

    public RequestBuilder WithUrl(Uri requestUri)
    {
        this.RenderedUri = RequestUriRenderer.RenderUri(requestUri);
        return this;
    }
    
    public RequestBuilder WithUrl(string requestUri)
    {
        this.RenderedUri = requestUri.AsMemory();
        return this;
    }

    public RequestBuilder WithEncodedForm(RequestEncodedKeyValueListBodyBuilder keyValueListBodyBuilder)
    {
        this.RenderedBody = RequestBodyRenderer.RenderKeyValueList(keyValueListBodyBuilder);
        return this;
    }
    
    public RequestBuilder WithForm(RequestKeyValueListBodyBuilder keyValueListBodyBuilder)
    {
        this.RenderedBody = RequestBodyRenderer.RenderKeyValueList(keyValueListBodyBuilder);
        return this;
    }

    public RequestBuilder WithRaw(RequestRawBodyBuilder rawBodyBuilder)
    {
        this.RenderedBody = RequestBodyRenderer.RenderBody(rawBodyBuilder);
        return this;
    }

    public RequestBuilder WithParameters(ParameterBuilder parameterBuilder)
    {
        this.ParameterBuilder = parameterBuilder;
        return this;
    }
    
    public RequestBuilder WithHeaders(RequestHeaderBuilder requestHeaderBuilder)
    {
        this.RequestHeaderBuilder = requestHeaderBuilder;
        return this;
    }
    
    protected override Action BuildAction => () =>
    {
        if (this.RenderedUri.IsEmpty)
            throw new ElementValidationException(EnumValidationReason.Empty);

        if (this.RequestHeaderBuilder == null)
            this.RequestHeaderBuilder = new RequestHeaderBuilder();
        
        this.RequestHeaderBuilder.Build();
        this.ParameterBuilder?.Build();
        
        if (this.ParameterBuilder == null)
            return;

        if (this.ParameterBuilder.Parameters.IsEmpty)
            return;
        
        StringBuilder newUrlBuilder =
            new StringBuilder(this.RenderedUri.Length + this.ParameterBuilder.Parameters.Length);

        newUrlBuilder.Append(this.RenderedUri);
        newUrlBuilder.Append(this.ParameterBuilder.Parameters);
        
        this.RenderedUri = newUrlBuilder.ToString().AsMemory();
    };
    
    public ReadOnlySpan<byte> Body => this.RenderedBody.Span;
    public ReadOnlySpan<char> Uri => this.RenderedUri.Span;
}