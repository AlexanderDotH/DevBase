using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DevBase.Generics;
using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Data.Header.UserAgent;

public class UserAgentHeaderBuilder : BogusHttpHeaderBuilder<UserAgentHeaderBuilder>
{
    [AllowNull]
    private string _productName;
    
    [AllowNull]
    private string _productVersion;
    
    private AList<IBogusUserAgentGenerator> BogusUserAgentGenerators { get; set; }

    public UserAgentHeaderBuilder(params IBogusUserAgentGenerator[] agentGenerators)
    {
        BogusUserAgentGenerators = new AList<IBogusUserAgentGenerator>(agentGenerators);
        
        if (agentGenerators.Length == 0)
        {
            BogusUserAgentGenerators.AddRange(
                new BogusChromeUserAgentGenerator(), 
                new BogusEdgeUserAgentGenerator(), 
                new BogusFirefoxUserAgentGenerator(), 
                new BogusOperaUserAgentGenerator());
        }
    }
    
    public UserAgentHeaderBuilder AddProductName(string productName)
    {
        this._productName = productName;
        return this;
    }

    public UserAgentHeaderBuilder AddProductVersion(string productVersion)
    {
        this._productVersion = productVersion;
        return this;
    }

    public UserAgentHeaderBuilder With(string userAgent)
    {
        this.HeaderStringBuilder.Clear();
        this.HeaderStringBuilder.Append(userAgent);
        return this;
    }

    public static UserAgentHeaderBuilder BogusOf(params IBogusUserAgentGenerator[] bogusGenerators)
    {
        return new UserAgentHeaderBuilder(bogusGenerators);
    }
    
    protected override Action BuildAction => () =>
    {
        if (this.HeaderStringBuilder.Length != 0)
            return;
        
        Assembly assembly = typeof(DevBase.Requests.Request).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        
        ReadOnlySpan<char> productName = this._productName;
        ReadOnlySpan<char> productVersion = this._productVersion;
        
        if (productName.IsEmpty)
            productName = assemblyName.Name;

        if (productVersion.IsEmpty)
            productVersion = assemblyName.Version!.ToString();

        this.HeaderStringBuilder.Append(productName);
        this.HeaderStringBuilder.Append('/');
        this.HeaderStringBuilder.Append(productVersion);
    };

    protected override Action BogusBuildAction => () =>
    {
        IBogusUserAgentGenerator userAgentGenerator = BogusUserAgentGenerators.GetRandom(BogusUtils.Random);
        
        if (this.AlreadyBuilt)
        {
            this.HeaderStringBuilder.Append(' ');
            this.HeaderStringBuilder.Append('a');
            this.HeaderStringBuilder.Append('s');
            this.HeaderStringBuilder.Append(' ');
            this.HeaderStringBuilder.Append(userAgentGenerator.UserAgentPart);
        }
        else
        {
            this.HeaderStringBuilder.Append(userAgentGenerator.UserAgentPart);
        }
    };

    public ReadOnlySpan<char> UserAgent
    {
        get => this.HeaderStringBuilder.ToString();
    }
}