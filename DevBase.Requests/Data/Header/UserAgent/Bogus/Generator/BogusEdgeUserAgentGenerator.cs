using System.Text;
using DevBase.Requests.Constants;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;

public class BogusEdgeUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly BogusChromeUserAgentGenerator _chromeUserAgentGenerator = new();
    
    public ReadOnlySpan<char> UserAgentPart => Generate().UserAgent;
    
    public UserAgentMetadata Generate()
    {
        StringBuilder edgeUserAgent = new StringBuilder(100);
        
        UserAgentMetadata chromeMetadata = _chromeUserAgentGenerator.Generate();

        string edgeVersion = BogusUtils.RandomVersion(
            minMajor: 100, maxMajor: 131, 
            useSubVersion: true, minSubVersion: 0, maxSubVersion: 0, 
            useMinor: true, minMinor: 100, maxMinor: 900, 
            usePatch: true, minPatch: 30, maxPatch: 60).ToString();
        
        // Mozilla/5.0 (Windows NT 4.1; Win64) AppleWebKit/416.3 (KHTML, like Gecko) Chrome/74.5.5678.737 Safari/416.3
        edgeUserAgent.Append(chromeMetadata.UserAgent);
        edgeUserAgent.Append(' ');
        
        // Edg/46.6.785.54
        edgeUserAgent.Append(UserAgentConstants.Edge.Span);
        edgeUserAgent.Append('/');
        edgeUserAgent.Append(edgeVersion);
        
        return new UserAgentMetadata
        {
            UserAgent = edgeUserAgent.ToString(),
            BrowserVersion = edgeVersion.Split('.')[0],
            ChromiumVersion = chromeMetadata.ChromiumVersion,
            Platform = chromeMetadata.Platform,
            IsMobile = chromeMetadata.IsMobile
        };
    }
}