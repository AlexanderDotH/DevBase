using System.Buffers;
using System.Reflection;
using System.Text;
using DevBase.Generics;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Extensions;
using DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent;

public class UserAgentHeaderBuilder
{
    private StringBuilder _preGeneratedUserAgent;

    private string _productName;
    private string _productVersion;

    private static AList<IBogusUserAgentGenerator> _bogusUserAgentGenerators;

    private bool _alreadyBuilded;
    
    static UserAgentHeaderBuilder()
    {
        _bogusUserAgentGenerators = new AList<IBogusUserAgentGenerator>(
            new BogusChromeUserAgentGenerator(), 
            new BogusEdgeUserAgentGenerator(), 
            new BogusFirefoxUserAgentGenerator(), 
            new BogusOperaUserAgentGenerator());
    }
    
    public UserAgentHeaderBuilder()
    {
        this._preGeneratedUserAgent = new StringBuilder();

        this._alreadyBuilded = false;
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

    public UserAgentHeaderBuilder Build()
    {
        if (this._alreadyBuilded)
            throw new HttpHeaderException("UserAgentHeader is already built");
                
        Assembly assembly = typeof(DevBase.Requests.Request).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        
        ReadOnlySpan<char> productName = this._productName;
        ReadOnlySpan<char> productVersion = this._productVersion;
        
        if (productName.IsEmpty)
            productName = assemblyName.Name;

        if (productVersion.IsEmpty)
            productVersion = assemblyName.Version!.ToString();

        this._preGeneratedUserAgent.Append(productName);
        this._preGeneratedUserAgent.Append('/');
        this._preGeneratedUserAgent.Append(productVersion);

        this._alreadyBuilded = true;
        
        return this;
    }

    public UserAgentHeaderBuilder BuildBogus()
    {
        IBogusUserAgentGenerator userAgentGenerator = _bogusUserAgentGenerators.GetRandom(BogusUtils.Random);

        if (this._alreadyBuilded)
        {
            this._preGeneratedUserAgent.Append(' ');
            this._preGeneratedUserAgent.Append('a');
            this._preGeneratedUserAgent.Append('s');
            this._preGeneratedUserAgent.Append(' ');
            this._preGeneratedUserAgent.Append(userAgentGenerator.UserAgentPart);
        }
        else
        {
            this._preGeneratedUserAgent.Append(userAgentGenerator.UserAgentPart);
        }
        
        this._alreadyBuilded = true;
        return this;
    }

    public ReadOnlySpan<char> UserAgent
    {
        get
        {
            char[] userAgent = Array.Empty<char>();
            this._preGeneratedUserAgent.ToSpan(ref userAgent);
            return userAgent;
        }
    }
}