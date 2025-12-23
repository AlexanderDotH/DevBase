using System.Text;
using DevBase.Requests.Constants;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;

public class BogusChromeUserAgentGenerator : IBogusUserAgentGenerator
{
    public ReadOnlySpan<char> UserAgentPart => Generate().UserAgent;
    
    public UserAgentMetadata Generate()
    {
        StringBuilder chromeUserAgent = new StringBuilder(100);

        ReadOnlySpan<char> product = BogusUtils.RandomProductName();
        ReadOnlySpan<char> productVersion = BogusUtils.RandomProductVersion();
        
        string chromeVersion = BogusUtils.RandomVersion(
            minMajor: 100, maxMajor: 131, 
            useSubVersion: true, minSubVersion: 0, maxSubVersion: 0, 
            useMinor: true, minMinor: 1000, maxMinor: 9000, 
            usePatch: true, minPatch: 1, maxPatch: 200).ToString();

        PlatformID platformId = BogusUtils.RandomPlatformId();
        string osPlatform = BogusUtils.RandomOperatingSystem(platformId).ToString();
        
        ReadOnlyMemory<char> platform = platformId switch
        {
            PlatformID.Win32NT => PlatformConstants.Windows,
            PlatformID.Unix => PlatformConstants.Linux,
            PlatformID.MacOSX => PlatformConstants.MacOS,
            _ => PlatformConstants.Windows
        };

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
        chromeUserAgent.Append(UserAgentConstants.AppleWebKit.Span);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(webKitVersion);
        chromeUserAgent.Append(' ');
        
        // (KHTML, like Gecko)
        chromeUserAgent.Append(UserAgentConstants.KhtmlLikeGecko.Span);
        chromeUserAgent.Append(' ');

        // Chrome/37.0.2062.94
        chromeUserAgent.Append(UserAgentConstants.Chrome.Span);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(chromeVersion);
        chromeUserAgent.Append(' ');

        // Safari/537.36
        chromeUserAgent.Append(UserAgentConstants.Safari.Span);
        chromeUserAgent.Append('/');
        chromeUserAgent.Append(webKitVersion);

        string majorVersion = chromeVersion.Split('.')[0];
        
        return new UserAgentMetadata
        {
            UserAgent = chromeUserAgent.ToString(),
            BrowserVersion = majorVersion,
            ChromiumVersion = majorVersion,
            Platform = platform.ToString(),
            IsMobile = false
        };
    }
    
    private ReadOnlySpan<char> RandomWebKitVersion()
    {
        StringBuilder webKitVersion = new StringBuilder();
        
        webKitVersion.Append(BogusUtils.RandomNumber(500, 650));
        webKitVersion.Append('.');
        webKitVersion.Append(BogusUtils.RandomNumber(1, 100));

        return webKitVersion.ToString();
    }
}