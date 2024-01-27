using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusEdgeUserAgentGenerator : IBogusUserAgentGenerator
{
    private static BogusChromeUserAgentGenerator _chromeUserAgentGenerator;
    
    private static readonly char[] _edgeTag;
    
    static BogusEdgeUserAgentGenerator()
    {
        _chromeUserAgentGenerator = new BogusChromeUserAgentGenerator();
        
        _edgeTag = "Edg".ToCharArray();
    }
    
    private ReadOnlySpan<char> BogusEdgeUserAgent()
    {
        StringBuilder edgeUserAgent = new StringBuilder(100);
        
        ReadOnlySpan<char> chromeUserAgent = _chromeUserAgentGenerator.UserAgentPart;

        ReadOnlySpan<char> edgeTag = _edgeTag;
        ReadOnlySpan<char> randomEdgeVersion = BogusUtils.RandomVersion(
            minMajor: 50, maxMajor: 90, 
            useSubVersion: true, minSubVersion: 1, maxSubVersion: 9, 
            useMinor: true, minMinor: 100, maxMinor: 900, 
            usePatch: true, minPatch: 30, maxPatch: 60);
        
        // Mozilla/5.0 (Windows NT 4.1; Win64) AppleWebKit/416.3 (KHTML, like Gecko) Chrome/74.5.5678.737 Safari/416.3
        edgeUserAgent.Append(chromeUserAgent);
        edgeUserAgent.Append(' ');
        
        // Edg/46.6.785.54
        edgeUserAgent.Append(edgeTag);
        edgeUserAgent.Append('/');
        edgeUserAgent.Append(randomEdgeVersion);
        
        char[] userAgent = Array.Empty<char>();
        edgeUserAgent.ToSpan(ref userAgent);

        return userAgent;
    }

    public ReadOnlySpan<char> UserAgentPart => BogusEdgeUserAgent();
}