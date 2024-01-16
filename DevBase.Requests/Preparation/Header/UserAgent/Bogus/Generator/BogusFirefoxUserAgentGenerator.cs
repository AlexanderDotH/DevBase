using System.Text;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusFirefoxUserAgentGenerator : IBogusUserAgentGenerator
{
    private StringBuilder _generatedUserAgentPart;
    
    public BogusFirefoxUserAgentGenerator()
    {
        this._generatedUserAgentPart = new StringBuilder();
        
        GenerateUserAgent();
    }

    private void GenerateUserAgent()
    {
        
    }
    
    public ReadOnlySpan<char> UserAgentPart { get; }
}