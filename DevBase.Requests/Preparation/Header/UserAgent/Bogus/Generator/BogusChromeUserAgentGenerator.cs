/*using System.Buffers;
using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;


public class BogusChromeUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly char[] _platformTag;
    private static readonly char[] _compatibilityTag;
    private static readonly char[] _chromeProduct;

    static BogusChromeUserAgentGenerator()
    {
        _platformTag = "AppleWebKit".ToCharArray();
        _compatibilityTag = "(KHTML, like Gecko)".ToCharArray();
        _chromeProduct = "Chrome".ToCharArray();
    }
    
    private ReadOnlySpan<char> BogusFirefoxUserAgent()
    {
        StringBuilder firefoxUserAgent = new StringBuilder();

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();

        PlatformID platformId = BogusUtils.RandomPlatformId();

        ReadOnlySpan<char> osPlatform = BogusUtils.RandomOperatingSystem(platformId);

        ReadOnlySpan<char> firefoxVersion = RandomFirefoxVersion();

        ReadOnlySpan<char> platformTag = _platformTag;
        ReadOnlySpan<char> compatibilityTag = _compatibilityTag;
        ReadOnlySpan<char> chromeProduct = _chromeProduct;

        // Mozilla/5.0
        firefoxUserAgent.Append(product);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(productVersion);
        firefoxUserAgent.Append(' ');

        // (Windows NT 6.1; Win64; rv:42.0)
        firefoxUserAgent.Append('(');
        firefoxUserAgent.Append(osPlatform);
        firefoxUserAgent.Append(' ');
        firefoxUserAgent.Append('r');
        firefoxUserAgent.Append('v');
        firefoxUserAgent.Append(':');
        firefoxUserAgent.Append(firefoxVersion);
        firefoxUserAgent.Append(')');
        firefoxUserAgent.Append(' ');
        
        // AppleWebKit/20100101
        firefoxUserAgent.Append(platformTag);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(geckoTrail);
        firefoxUserAgent.Append(' ');

        // Firefox/42.0
        firefoxUserAgent.Append(firefoxProduct);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(firefoxVersion);

        char[] userAgent = Array.Empty<char>();
        firefoxUserAgent.ToSpan(ref userAgent);

        return userAgent;
    }
    
    private ReadOnlySpan<char> RandomVersion(
        int minMajor = 40, 
        int maxMajor = 121, 
        int minMinor = 0, 
        int maxMinor = 99, 
        int minPatch = 0, 
        int maxPatch = 99, 
        bool useMinor = true, 
        bool usePatch = false)
    {
        StringBuilder randomVersionBuilder = new StringBuilder();
        
        ReadOnlySpan<char> randomMajor = BogusUtils.Random.Next(minMajor, maxMajor).ToString();
        ReadOnlySpan<char> randomMinor = null;

        if (useMinor)
            randomMinor = BogusUtils.Random.Next(minMinor, maxMinor).ToString();
        
        
        ReadOnlySpan<char> randomSubVersion = BogusUtils.Random.Next(1, 99).ToString();

        
        
        char[] version = Array.Empty<char>();
        //firefoxVersion.ToSpan(ref version);

        return version;
    }
    
    public ReadOnlySpan<char> UserAgentPart => BogusFirefoxUserAgent();
}*/