using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusFirefoxUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly char[] _firefoxProduct;
    private static readonly char[] _geckoEngine;
    private static readonly char[] _geckoTrail;

    static BogusFirefoxUserAgentGenerator()
    {
        _firefoxProduct = "Firefox".ToCharArray();
        _geckoEngine = "Gecko".ToCharArray();
        _geckoTrail = "20100101".ToCharArray();
    }
    
    private ReadOnlySpan<char> BogusFirefoxUserAgent()
    {
        StringBuilder firefoxUserAgent = new StringBuilder();

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();

        PlatformID platformId = BogusUtils.RandomPlatformId();

        ReadOnlySpan<char> osPlatform = BogusUtils.RandomOperatingSystem(platformId);

        ReadOnlySpan<char> firefoxVersion = RandomFirefoxVersion();

        ReadOnlySpan<char> firefoxProduct = _firefoxProduct;
        ReadOnlySpan<char> geckoEngine = _geckoEngine;
        ReadOnlySpan<char> geckoTrail = _geckoTrail;

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
        
        // Gecko/20100101
        firefoxUserAgent.Append(geckoEngine);
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
    
    private ReadOnlySpan<char> RandomFirefoxVersion()
    {
        StringBuilder firefoxVersion = new StringBuilder();
        
        ReadOnlySpan<char> randomVersion = BogusUtils.Random.Next(40, 121).ToString();

        firefoxVersion.Append(randomVersion);
        firefoxVersion.Append('.');
        firefoxVersion.Append('0');

        char[] version = Array.Empty<char>();
        firefoxVersion.ToSpan(ref version);

        return version;
    }
    
    public ReadOnlySpan<char> UserAgentPart => BogusFirefoxUserAgent();
}