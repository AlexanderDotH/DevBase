using System.Text;
using DevBase.Net.Constants;
using DevBase.Net.Utils;

namespace DevBase.Net.Data.Header.UserAgent.Bogus.Generator;

public class BogusFirefoxUserAgentGenerator : IBogusUserAgentGenerator
{
    
    public ReadOnlySpan<char> UserAgentPart => Generate().UserAgent;
    
    public UserAgentMetadata Generate()
    {
        StringBuilder firefoxUserAgent = new StringBuilder();

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();

        PlatformID platformId = BogusUtils.RandomPlatformId();
        string osPlatform = BogusUtils.RandomOperatingSystem(platformId).ToString();
        
        ReadOnlyMemory<char> platform = platformId switch
        {
            PlatformID.Win32NT => PlatformConstants.Windows,
            PlatformID.Unix => PlatformConstants.Linux,
            PlatformID.MacOSX => PlatformConstants.MacOS,
            _ => PlatformConstants.Windows
        };
        
        string firefoxMajor = new string(BogusUtils.RandomNumber(100, 133));
        StringBuilder versionBuilder = new StringBuilder(8);
        versionBuilder.Append(firefoxMajor);
        versionBuilder.Append(".0");
        string firefoxVersion = versionBuilder.ToString();

        // Mozilla/5.0
        firefoxUserAgent.Append(product);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(productVersion);
        firefoxUserAgent.Append(' ');

        // (Windows NT 6.1; Win64; rv:42.0)
        firefoxUserAgent.Append('(');
        firefoxUserAgent.Append(osPlatform);
        firefoxUserAgent.Append(UserAgentConstants.RvPrefix.Span);
        firefoxUserAgent.Append(firefoxVersion);
        firefoxUserAgent.Append(')');
        firefoxUserAgent.Append(' ');
        
        // Gecko/20100101
        firefoxUserAgent.Append(UserAgentConstants.Gecko.Span);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(UserAgentConstants.GeckoTrail.Span);
        firefoxUserAgent.Append(' ');

        // Firefox/42.0
        firefoxUserAgent.Append(UserAgentConstants.Firefox.Span);
        firefoxUserAgent.Append('/');
        firefoxUserAgent.Append(firefoxVersion);

        return new UserAgentMetadata
        {
            UserAgent = firefoxUserAgent.ToString(),
            BrowserVersion = firefoxMajor,
            ChromiumVersion = null,
            Platform = platform.ToString(),
            IsMobile = false
        };
    }
}