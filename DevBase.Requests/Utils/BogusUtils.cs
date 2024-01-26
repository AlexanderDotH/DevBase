using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using DevBase.Requests.Extensions;

namespace DevBase.Requests.Utils;

public class BogusUtils
{
    private static readonly (char[] Value, PlatformID PlatformId)[] _desktopOperatingSystems;
    private static readonly (char[] Value, PlatformID PlatformId)[] _desktopOperatingSystemsArchitecture;

    private static readonly char[][] _product;
    private static readonly char[][] _productVersion;
    private static readonly char[] _randomNumberRange;

    private static Random _random;
    
    static BogusUtils()
    {
        _desktopOperatingSystems = new (char[] Value, PlatformID PlatformId)[]
        {
            ("Windows NT".ToCharArray(), PlatformID.Win32NT), // Windows also has a version after like "6.1"
            ("Macintosh".ToCharArray(), PlatformID.MacOSX),
            ("X11".ToCharArray(), PlatformID.Unix)
        };
        
        _desktopOperatingSystemsArchitecture = new (char[] Value, PlatformID PlatformId)[]
        {
            ("Win64".ToCharArray(), PlatformID.Win32NT),
            ("Win32".ToCharArray(), PlatformID.Win32NT),
            ("Intel Mac OS X".ToCharArray(), PlatformID.MacOSX), // Mac also has a version agter it "14_2_1"
            ("Linux x86_64".ToCharArray(), PlatformID.Unix),
            ("Linux i686".ToCharArray(), PlatformID.Unix)
        };

        _product = new[]
        {
            "Mozilla".ToCharArray()
        };
        
        _productVersion = new[]
        {
            "5.0".ToCharArray()
        };

        _randomNumberRange = new[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        
        _random = new Random();
    }

    public static PlatformID RandomPlatformId()
    {
        int osType = _random.Next(1, 3) * 2;
        return (PlatformID)osType;
    }

    public static ReadOnlySpan<char> RandomOS(PlatformID platformId)
    {
        (char[] Value, PlatformID PlatformId)[] available = _desktopOperatingSystems
            .Where(c => c.PlatformId == platformId)
            .ToArray();

        return available[_random.Next(0, available.Length)].Value;
    }
    
    public static ReadOnlySpan<char> RandomArchitecture(PlatformID platformId)
    {
        (char[] Value, PlatformID PlatformId)[] available = _desktopOperatingSystemsArchitecture
            .Where(c => c.PlatformId == platformId)
            .ToArray();

        return available[_random.Next(0, available.Length)].Value;
    }

    public static ReadOnlySpan<char> RandomProductName()
    {
        return _product[_random.Next(_product.Length)];
    }
    
    public static ReadOnlySpan<char> RandomProductVersion()
    {
        return _productVersion[_random.Next(_productVersion.Length)];
    }
    
    public static ReadOnlySpan<char> RandomNTVersion()
    {
        StringBuilder ntBuilder = new StringBuilder();

        ReadOnlySpan<char> major = _random.Next(1, 10).ToString();
        ReadOnlySpan<char> minor = _random.Next(1, 9).ToString();

        ntBuilder.Append(major);
        ntBuilder.Append('.');
        ntBuilder.Append(minor);

        char[] ntVersion = Array.Empty<char>();
        ntBuilder.ToSpan(ref ntVersion);

        return ntVersion;
    }

    public static ReadOnlySpan<char> RandomIOSVersion()
    {
        StringBuilder iosBuilder = new StringBuilder();

        ReadOnlySpan<char> major = _random.Next(1, 17).ToString();
        ReadOnlySpan<char> minor = _random.Next(1, 9).ToString();
        ReadOnlySpan<char> build = _random.Next(1, 9).ToString();

        iosBuilder.Append(major);
        iosBuilder.Append('_');
        iosBuilder.Append(minor);
        iosBuilder.Append('_');
        iosBuilder.Append(build);

        char[] iosVersion = Array.Empty<char>();
        iosBuilder.ToSpan(ref iosVersion);

        return iosVersion;
    }

    public static ReadOnlySpan<char> RandomNumber(int min, int max)
    {
        if (min > max)
            throw new System.Exception("Min is bigger than max");
        
        StringBuilder numberBuilder = new StringBuilder();

        int randomNumber = _random.Next(min, max);

        int pre = randomNumber;

        while (pre > 0)
        {
            double x = pre / 10.0;
            int y = (int)x;
            int z = (int)(x - y) * 10;

            numberBuilder.Append(_randomNumberRange[z]);
            
            pre = y;
        }
        
        char[] number = Array.Empty<char>();
        numberBuilder.ToSpan(ref number);

        return number;
    }
    
    public static ReadOnlySpan<char> RandomOperatingSystem(PlatformID platformId)
    {
        StringBuilder osStringBuilder = new StringBuilder();
        
        ReadOnlySpan<char> operatingSystem = RandomOS(platformId);
        ReadOnlySpan<char> architecture = RandomArchitecture(platformId);

        osStringBuilder.Append(operatingSystem);
        
        switch (platformId)
        {
            // Windows NT 6.1; Win64;
            case PlatformID.Win32NT:
            {
                osStringBuilder.Append(' ');
                osStringBuilder.Append(RandomNTVersion());
                osStringBuilder.Append(';');
                osStringBuilder.Append(' ');
                osStringBuilder.Append(architecture);
                osStringBuilder.Append(';');

                break;
            }
            // Macintosh; Intel Mac OS X 10_9_2;
            case PlatformID.MacOSX:
            {
                osStringBuilder.Append(';');
                osStringBuilder.Append(' ');
                osStringBuilder.Append(architecture);
                osStringBuilder.Append(' ');
                osStringBuilder.Append(RandomIOSVersion());
                osStringBuilder.Append(';');
                break;
            }
            // X11; Linux x86_64
            case PlatformID.Unix:
            {
                osStringBuilder.Append(';');
                osStringBuilder.Append(' ');
                osStringBuilder.Append(architecture);
                osStringBuilder.Append(';');
                break;
            }
        }
        
        char[] os = Array.Empty<char>();
        osStringBuilder.ToSpan(ref os);

        return os;
    }

    public static Random Random
    {
        get => _random;
    }
}