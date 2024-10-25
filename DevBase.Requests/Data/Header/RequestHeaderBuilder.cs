using System.Runtime.CompilerServices;
using System.Text;
using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Body.Mime;
using DevBase.Requests.Data.Header.Authentication;
using DevBase.Requests.Data.Header.UserAgent;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Utilities;

namespace DevBase.Requests.Data.Header;

public class RequestHeaderBuilder : HttpKeyValueListBuilder<RequestHeaderBuilder, string, string>
{
    private UserAgentHeaderBuilder? UserAgentHeaderBuilder { get; set; }
    private AuthenticationHeaderBuilder? AuthenticationHeaderBuilder { get; set; }
    
    private MimeDictionary MimeDictionary { get; set; }

    public RequestHeaderBuilder()
    {
        this.UserAgentHeaderBuilder = new UserAgentHeaderBuilder();
        this.AuthenticationHeaderBuilder = new AuthenticationHeaderBuilder();

        this.MimeDictionary = new MimeDictionary();
    }
    
    #region UserAgent

    public RequestHeaderBuilder WithUserAgent(string userAgent)
    {
        this.UserAgentHeaderBuilder?.WithOverwrite(userAgent);
        return this;
    }
    
    public RequestHeaderBuilder WithUserAgent(UserAgentHeaderBuilder agentHeaderBuilder)
    {
        this.UserAgentHeaderBuilder = agentHeaderBuilder;
        return this;
    }

    public RequestHeaderBuilder WithBogusUserAgent()
    {
        this.UserAgentHeaderBuilder?.BuildBogus();
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
            string currentTyoe = acceptTypes[i];

            if (this.MimeDictionary.TryGetMimeTypeAsString(currentTyoe, out string result))
                currentTyoe = result;
            
            resolvedTypes[i] = currentTyoe;
        }
        
        string combined = StringUtils.Separate(resolvedTypes);
        
        base.AddEntry("Accept", combined);
        return this;
    }

    #endregion

    #region Authentication

    public RequestHeaderBuilder UseBasicAuthentication(string username, string password)
    {
        AuthenticationHeaderBuilder?.UseBasicAuthentication(username, password);
        return this;
    }

    public RequestHeaderBuilder UseBearerAuthentication(string token)
    {
        AuthenticationHeaderBuilder?.UseBearerAuthentication(token);
        return this;
    }
    
    #endregion
    
    protected override Action BuildAction => () =>
    {
        this.UserAgentHeaderBuilder?.TryBuild();
        this.AuthenticationHeaderBuilder?.TryBuild(); 

        if (!base.AnyEntry("Accept"))
            WithAccept("*/*");

        if (this.UserAgentHeaderBuilder!.Usable)
            base.AddOrSetEntry("User-Agent", this.UserAgentHeaderBuilder.UserAgent.ToString());

        if (this.AuthenticationHeaderBuilder!.Usable)
        {
            string headerKey = this.AuthenticationHeaderBuilder.AuthenticationKey.ToString();
            string headerValue = this.AuthenticationHeaderBuilder.AuthenticationValue.ToString();
            
            base.AddOrSetEntry(headerKey, headerValue);
        }

    };

    /**
     * Some accessibility functions.
     * Perhaps you could use modern C# but some people are just the getter and setter type of person. ;)
    **/ 
    public string GetHeader(string key) => this[key];
    public string GetHeader(int index) => this[index];
    
    public string SetHeader(string key, string value) => this[key] = value;
    public string SetHeader(int index, string value) => this[index] = value;
    
    public string this[string key]
    {
        get => base.GetEntryValue(key);
        set => base.SetEntryValue(key, value);
    }
    
    public string this[int index]
    {
        get => base.GetEntryValue(index);
        set => base.SetEntryValue(index, value);
    }
}