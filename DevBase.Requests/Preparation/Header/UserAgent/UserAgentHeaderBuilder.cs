using System.Text;

namespace DevBase.Requests.Preparation.Header.UserAgent;

public class UserAgentHeaderBuilder
{
    private StringBuilder _preGeneratedUserAgent;

    private string _productName;
    
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
        
    }

    public UserAgentHeaderBuilder Build()
    {
        return this;
    }
}