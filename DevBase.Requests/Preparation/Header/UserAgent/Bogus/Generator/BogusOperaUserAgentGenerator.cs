using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusOperaUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly BogusChromeUserAgentGenerator _chromeUserAgentGenerator = new();
    private static readonly char[] _operaTag = "OPR".ToCharArray();
    
    private ReadOnlySpan<char> BogusOperaUserAgent()
    {
        StringBuilder operaUserAgent = new StringBuilder(100);
        
        ReadOnlySpan<char> chromeUserAgent = _chromeUserAgentGenerator.UserAgentPart;

        ReadOnlySpan<char> operaTag = _operaTag;
        ReadOnlySpan<char> randomOperaVersion = BogusUtils.RandomVersion(
            minMajor: 50, maxMajor: 90, 
            useSubVersion: true, minSubVersion: 1, maxSubVersion: 9, 
            useMinor: true, minMinor: 100, maxMinor: 900, 
            usePatch: true, minPatch: 30, maxPatch: 60);
        
        // Mozilla/5.0 (Windows NT 4.1; Win64) AppleWebKit/416.3 (KHTML, like Gecko) Chrome/74.5.5678.737 Safari/416.3
        operaUserAgent.Append(chromeUserAgent);
        operaUserAgent.Append(' ');
        
        // OPR/46.6.785.54
        operaUserAgent.Append(operaTag);
        operaUserAgent.Append('/');
        operaUserAgent.Append(randomOperaVersion);
        
        return operaUserAgent.ToString();
    }

    public ReadOnlySpan<char> UserAgentPart => BogusOperaUserAgent();
}