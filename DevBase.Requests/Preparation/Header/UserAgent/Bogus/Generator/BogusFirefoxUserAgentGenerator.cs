using System.Text;
using DevBase.Requests.Extensions;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus.Generator;

public class BogusFirefoxUserAgentGenerator : IBogusUserAgentGenerator
{
    private static readonly char[] _firefoxProduct = "Firefox".ToCharArray();
    private static readonly char[] _geckoEngine = "Gecko".ToCharArray();
    private static readonly char[] _geckoTrail = "20100101".ToCharArray();
    
    private ReadOnlySpan<char> BogusFirefoxUserAgent()
    {
        StringBuilder firefoxUserAgent = new StringBuilder();

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();

        PlatformID platformId = BogusUtils.RandomPlatformId();

        ReadOnlySpan<char> osPlatform = BogusUtils.RandomOperatingSystem(platformId);

        ReadOnlySpan<char> firefoxVersion = RandomFirefoxVersion();

        // Mozilla/5.0
        firefoxUserAgent.Append(product);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(productVersion);
        firefoxUserAgent.Append(' ');

        // (Windows NT 6.1; Win64; rv:42.0)
        firefoxUserAgent.Append('(');
        firefoxUserAgent.Append(osPlatform);
        firefoxUserAgent.Append(';');
        firefoxUserAgent.Append(' ');
        firefoxUserAgent.Append('r');
        firefoxUserAgent.Append('v');
        firefoxUserAgent.Append(':');
        firefoxUserAgent.Append(firefoxVersion);
        firefoxUserAgent.Append(')');
        firefoxUserAgent.Append(' ');
        
        // Gecko/20100101
        firefoxUserAgent.Append(_geckoEngine);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(_geckoTrail);
        firefoxUserAgent.Append(' ');

        // Firefox/42.0
        firefoxUserAgent.Append(_firefoxProduct);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(firefoxVersion);

        return firefoxUserAgent.ToString();
    }
    
    private ReadOnlySpan<char> RandomFirefoxVersion()
    {
        StringBuilder firefoxVersion = new StringBuilder();
        
        firefoxVersion.Append(BogusUtils.RandomNumber(40, 121));
        firefoxVersion.Append('.');
        firefoxVersion.Append('0');

        return firefoxVersion.ToString();
    }
    
    public ReadOnlySpan<char> UserAgentPart => BogusFirefoxUserAgent();
}