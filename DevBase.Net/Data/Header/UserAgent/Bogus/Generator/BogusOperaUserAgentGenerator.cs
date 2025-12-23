using System.Text;
using DevBase.Net.Constants;
using DevBase.Net.Utils;

namespace DevBase.Net.Data.Header.UserAgent.Bogus.Generator;

public class BogusOperaUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly BogusChromeUserAgentGenerator _chromeUserAgentGenerator = new();
    
    public ReadOnlySpan<char> UserAgentPart => Generate().UserAgent;
    
    public UserAgentMetadata Generate()
    {
        StringBuilder operaUserAgent = new StringBuilder(100);
        
        UserAgentMetadata chromeMetadata = _chromeUserAgentGenerator.Generate();

        string operaVersion = BogusUtils.RandomVersion(
            minMajor: 100, maxMajor: 115, 
            useSubVersion: true, minSubVersion: 0, maxSubVersion: 0, 
            useMinor: true, minMinor: 100, maxMinor: 900, 
            usePatch: true, minPatch: 30, maxPatch: 60).ToString();
        
        operaUserAgent.Append(chromeMetadata.UserAgent);
        operaUserAgent.Append(' ');
        
        // OPR/46.6.785.54
        operaUserAgent.Append(UserAgentConstants.Opera.Span);
        operaUserAgent.Append('/');
        operaUserAgent.Append(operaVersion);
        
        return new UserAgentMetadata
        {
            UserAgent = operaUserAgent.ToString(),
            BrowserVersion = operaVersion.Split('.')[0],
            ChromiumVersion = chromeMetadata.ChromiumVersion,
            Platform = chromeMetadata.Platform,
            IsMobile = chromeMetadata.IsMobile
        };
    }
}