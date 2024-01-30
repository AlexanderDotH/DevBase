using System.Buffers;
using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusChromeUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly char[] _platformTag = "AppleWebKit".ToCharArray();
    private static readonly char[] _compatibilityTag = "(KHTML, like Gecko)".ToCharArray();
    private static readonly char[] _chromeProduct = "Chrome".ToCharArray();
    private static readonly char[] _safariProduct = "Safari".ToCharArray();
    
    private ReadOnlySpan<char> BogusChromeUserAgent()
    {
        StringBuilder chromeUserAgent = new StringBuilder(100);

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();
        
        ReadOnlySpan<char> chromeVersion = BogusUtils.RandomVersion(
            minMajor: 1, maxMajor: 60, 
            useSubVersion: true, minSubVersion: 1, maxSubVersion: 9, 
            useMinor: true, minMinor: 1000, maxMinor: 9000, 
            usePatch: true, minPatch: 1, maxPatch: 900);

        PlatformID platformId = BogusUtils.RandomPlatformId();

        ReadOnlySpan<char> osPlatform = BogusUtils.RandomOperatingSystem(platformId);

        ReadOnlySpan<char> webKitVersion = RandomWebKitVersion();

        // Mozilla/5.0
        chromeUserAgent.Append(product);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(productVersion);
        chromeUserAgent.Append(' ');

        // (Windows NT 6.1; Win64;)
        chromeUserAgent.Append('(');
        chromeUserAgent.Append(osPlatform);
        chromeUserAgent.Append(')');
        chromeUserAgent.Append(' ');
        
        // AppleWebKit/537.36
        chromeUserAgent.Append(_platformTag);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(webKitVersion);
        chromeUserAgent.Append(' ');
        
        // (KHTML, like Gecko)
        chromeUserAgent.Append(_compatibilityTag);
        chromeUserAgent.Append(' ');

        // Chrome/37.0.2062.94
        chromeUserAgent.Append(_chromeProduct);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(chromeVersion);
        chromeUserAgent.Append(' ');

        // Safari/537.36
        chromeUserAgent.Append(_safariProduct);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(webKitVersion);

        return chromeUserAgent.ToString();
    }
    
    private ReadOnlySpan<char> RandomWebKitVersion()
    {
        StringBuilder webKitVersion = new StringBuilder();
        
        webKitVersion.Append(BogusUtils.RandomNumber(500, 650));
        webKitVersion.Append('.');
        webKitVersion.Append(BogusUtils.RandomNumber(1, 100));

        return webKitVersion.ToString();
    }
   
    
    public ReadOnlySpan<char> UserAgentPart => BogusChromeUserAgent();
}