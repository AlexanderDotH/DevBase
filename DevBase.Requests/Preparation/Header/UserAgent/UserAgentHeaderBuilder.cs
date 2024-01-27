using System.Reflection;
using DevBase.Generics;
using DevBase.Requests.Abstract;
using DevBase.Requests.Extensions;
using DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent;

public class UserAgentHeaderBuilder : BogusHttpHeaderBuilder<UserAgentHeaderBuilder>
{
    private string _productName;
    private string _productVersion;

    private static AList<IBogusUserAgentGenerator> _bogusUserAgentGenerators;

    static UserAgentHeaderBuilder()
    {
        _bogusUserAgentGenerators = new AList<IBogusUserAgentGenerator>(
            new BogusChromeUserAgentGenerator(), 
            new BogusEdgeUserAgentGenerator(), 
            new BogusFirefoxUserAgentGenerator(), 
            new BogusOperaUserAgentGenerator());
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

    protected override Action BuildAction => () =>
    {
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
        IBogusUserAgentGenerator userAgentGenerator = _bogusUserAgentGenerators.GetRandom(BogusUtils.Random);
        
        if (this.AlreadyBuilded)
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
        get
        {
            char[] userAgent = Array.Empty<char>();
            this.HeaderStringBuilder.ToSpan(ref userAgent);
            return userAgent;
        }
    }
}