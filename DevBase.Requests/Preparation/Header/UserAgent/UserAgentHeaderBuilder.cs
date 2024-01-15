using System.Buffers;
using System.Reflection;
using System.Text;
using DevBase.Requests.Extensions;

namespace DevBase.Requests.Preparation.Header.UserAgent;

public class UserAgentHeaderBuilder
{
    private StringBuilder _preGeneratedUserAgent;

    private string _productName;
    private string _productVersion;
    
    public UserAgentHeaderBuilder()
    {
        this._preGeneratedUserAgent = new StringBuilder();
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