using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Header.Body.Mime;
using DevBase.Requests.Data.Header.UserAgent;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Utilities;

namespace DevBase.Requests.Data.Header;

public class RequestHeaderBuilder : HttpKeyValueListBuilder<RequestHeaderBuilder, string, string>
{
    private UserAgentHeaderBuilder UserAgentHeaderBuilder { get; set; }
    
    private MimeDictionary MimeDictionary { get; set; } 
    
    public RequestHeaderBuilder()
    {
        this.UserAgentHeaderBuilder = new UserAgentHeaderBuilder();

        this.MimeDictionary = new MimeDictionary();
    }

    #region UserAgent

    public RequestHeaderBuilder WithUserAgent(string userAgent)
    {
        this.UserAgentHeaderBuilder.With(userAgent);
        return this;
    }
    
    public RequestHeaderBuilder WithUserAgent(UserAgentHeaderBuilder agentHeaderBuilder)
    {
        this.UserAgentHeaderBuilder = agentHeaderBuilder;
        return this;
    }

    public RequestHeaderBuilder WithBogusUserAgent()
    {
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }

    #region Generics

    public RequestHeaderBuilder WithBogusUserAgent<T>() 
        where T : IBogusUserAgentGenerator 
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(CreateInstance<T>());
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    
    public RequestHeaderBuilder WithBogusUserAgent<T1, T2>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(
            CreateInstance<T1>(),
            CreateInstance<T2>());
        
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    
    #pragma warning disable S2436
    public RequestHeaderBuilder WithBogusUserAgent<T1, T2, T3>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
        where T3 : IBogusUserAgentGenerator
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(
            CreateInstance<T1>(),
            CreateInstance<T2>(),
            CreateInstance<T3>());
        
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    #pragma warning restore S2436
    
    #pragma warning disable S2436
    public RequestHeaderBuilder WithBogusUserAgent<T1, T2, T3, T4>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
        where T3 : IBogusUserAgentGenerator
        where T4 : IBogusUserAgentGenerator
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(
            CreateInstance<T1>(),
            CreateInstance<T2>(),
            CreateInstance<T3>(),
            CreateInstance<T4>());
        
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    #pragma warning restore S2436

    private IBogusUserAgentGenerator CreateInstance<T>() => (IBogusUserAgentGenerator)Activator.CreateInstance(typeof(T));
    
    #endregion
    
    #endregion

    #region Accept

    public RequestHeaderBuilder WithAccept(params string[] acceptTypes)
    {
        string[] resolvedTypes = new string[acceptTypes.Length];
        
        for (var i = 0; i < acceptTypes.Length; i++)
        {
            string result = acceptTypes[i];
            this.MimeDictionary.TryGetMimeTypeAsString(result, out result);
            resolvedTypes[i] = result;
        }
        
        string combined = StringUtils.Separate(resolvedTypes);
        
        this.AddHeader("Accept", combined);
        return this;
    }
    
    

    #endregion

    public RequestHeaderBuilder AddHeader(string name, string value)
    {
        this.AddEntry(name, value);
        return this;
    }
    
    protected override Action BuildAction => () =>
    {
        if (!UserAgentHeaderBuilder.AlreadyBuilt)
            this.UserAgentHeaderBuilder.Build();

        if (!this.AnyEntry("Accept"))
            WithAccept("*");
    };
}