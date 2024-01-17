using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusFirefoxUserAgentGenerator : IBogusUserAgentGenerator
{
    private ReadOnlySpan<char> BogusFirefoxUserAgent()
    {
        StringBuilder firefoxUserAgent = new StringBuilder();

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();

        PlatformID platformId = BogusUtils.RandomPlatformId();

        ReadOnlySpan<char> osPlatform = BogusUtils.RandomOperatingSystem(platformId);

        ReadOnlySpan<char> firefoxProduct = "Firefox";
        ReadOnlySpan<char> firefoxVersion = RandomFirefoxVersion();

        ReadOnlySpan<char> geckoEngine = "Gecko";
        ReadOnlySpan<char> geckoTrail = "20100101";

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