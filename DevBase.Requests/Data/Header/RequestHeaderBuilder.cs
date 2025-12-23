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
    private static readonly MimeDictionary SharedMimeDictionary = new();

    private UserAgentHeaderBuilder? UserAgentHeaderBuilder { get; set; }
    private AuthenticationHeaderBuilder? AuthenticationHeaderBuilder { get; set; }

    public RequestHeaderBuilder()
    {
        this.UserAgentHeaderBuilder = new UserAgentHeaderBuilder();
        this.AuthenticationHeaderBuilder = new AuthenticationHeaderBuilder();
    }
    
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

    public RequestHeaderBuilder WithBogusUserAgent<T>() 
        where T : IBogusUserAgentGenerator 
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(this.CreateInstance<T>());
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    
    public RequestHeaderBuilder WithBogusUserAgent<T1, T2>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(
            this.CreateInstance<T1>(),
            this.CreateInstance<T2>());
        
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    
    public RequestHeaderBuilder WithBogusUserAgent<T1, T2, T3>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
        where T3 : IBogusUserAgentGenerator
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(
            this.CreateInstance<T1>(),
            this.CreateInstance<T2>(),
            this.CreateInstance<T3>());
        
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }
    
    public RequestHeaderBuilder WithBogusUserAgent<T1, T2, T3, T4>() 
        where T1 : IBogusUserAgentGenerator 
        where T2 : IBogusUserAgentGenerator
        where T3 : IBogusUserAgentGenerator
        where T4 : IBogusUserAgentGenerator
    {
        this.UserAgentHeaderBuilder = UserAgentHeaderBuilder.BogusOf(
            this.CreateInstance<T1>(),
            this.CreateInstance<T2>(),
            this.CreateInstance<T3>(),
            this.CreateInstance<T4>());
        
        this.UserAgentHeaderBuilder.BuildBogus();
        return this;
    }

    private IBogusUserAgentGenerator CreateInstance<T>() => (IBogusUserAgentGenerator)Activator.CreateInstance(typeof(T));
    
    public RequestHeaderBuilder WithAccept(params string[] acceptTypes)
    {
        string[] resolvedTypes = new string[acceptTypes.Length];
        
        for (int i = 0; i < acceptTypes.Length; i++)
        {
            string currentTyoe = acceptTypes[i];

            if (SharedMimeDictionary.TryGetMimeTypeAsString(currentTyoe, out string result))
                currentTyoe = result;
            
            resolvedTypes[i] = currentTyoe;
        }
        
        string combined = StringUtils.Separate(resolvedTypes);
        
        base.AddEntry("Accept", combined);
        return this;
    }

    public RequestHeaderBuilder UseBasicAuthentication(string username, string password)
    {
        this.AuthenticationHeaderBuilder?.UseBasicAuthentication(username, password);
        return this;
    }

    public RequestHeaderBuilder UseBearerAuthentication(string token)
    {
        this.AuthenticationHeaderBuilder?.UseBearerAuthentication(token);
        return this;
    }
    
    protected override Action BuildAction => () =>
    {
        this.UserAgentHeaderBuilder?.TryBuild();
        this.AuthenticationHeaderBuilder?.TryBuild(); 

        if (!base.AnyEntry("Accept"))
            this.WithAccept("*/*");

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
        set => base.AddOrSetEntry(key, value);
    }
    
    public string this[int index]
    {
        get => base.GetEntryValue(index);
        set => base.SetEntryValue(index, value);
    }
}