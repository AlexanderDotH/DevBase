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
    
    public static ReadOnlySpan<char> RandomVersion(
        int minMajor = 40, 
        int maxMajor = 121, 
        int minSubVersion = 40, 
        int maxSubVersion = 121, 
        int minMinor = 0, 
        int maxMinor = 99, 
        int minPatch = 0, 
        int maxPatch = 99, 
        bool useSubVersion = true,
        bool useMinor = false, 
        bool usePatch = false,
        char separator = '.')
    {
        StringBuilder randomVersionBuilder = StringBuilderByVersioning(
            minMajor, maxMajor, minSubVersion, maxSubVersion,
            minMinor, maxMinor, minPatch, maxPatch);
        
        randomVersionBuilder.Append(RandomNumber(minMajor, maxMajor));

        if (useSubVersion)
        {
            randomVersionBuilder.Append(separator);
            randomVersionBuilder.Append(
                (minSubVersion == 0 && maxSubVersion == 0)
                ? new [] {'0'}
                : RandomNumber(minSubVersion, maxSubVersion));
        }
        
        if (useMinor)
        {
            randomVersionBuilder.Append(separator);
            
            randomVersionBuilder.Append(
                (minMinor == 0 && maxMinor == 0)
                    ? new [] {'0'}
                    : RandomNumber(minMinor, maxMinor));
        }
        
        if (usePatch)
        {
            randomVersionBuilder.Append(separator);
            
            randomVersionBuilder.Append(
                (minPatch == 0 && maxPatch == 0)
                    ? new [] {'0'}
                    : RandomNumber(minPatch, maxPatch));
        }
        
        char[] version = Array.Empty<char>();
        randomVersionBuilder.ToSpan(ref version);

        return version;
    }
    
    public static char[] RandomNumber(int min, int max)
    {
        if (min > max)
            throw new System.Exception("Min is bigger than max");
        
        int randomNumber = _random.Next(min, max);
        
        int length = GetDigits(randomNumber);
        
        char[] numberSpan = new char[length];

        for (int i = 0; i < length; i++)
        {
            numberSpan[i] = _randomNumberRange[randomNumber % 10];
            randomNumber = (int)(randomNumber * 0.1);
        }

        if (numberSpan[0] == '0')
            return RandomNumber(min, max);
        
        return numberSpan;
    }

    private static int GetDigits(int number)
    {
        switch (number)
        {
            case <= 9:
                return 1;
            case <= 99:
                return 2;
            case <= 999:
                return 3;
            case <= 9999:
                return 4;
            case <= 99999:
                return 5;
            case <= 999999:
                return 6;
            case <= 9999999:
                return 7;
            case <= 99999999:
                return 8;
            case <= 999999999:
                return 9;
            default:
                return 10;
        }
    }
    
    public static ReadOnlySpan<char> RandomOperatingSystem(PlatformID platformId)
    {
        StringBuilder osStringBuilder = StringBuilderFromPlatform(platformId);
        
        ReadOnlySpan<char> operatingSystem = RandomOS(platformId);
        ReadOnlySpan<char> architecture = RandomArchitecture(platformId);

        osStringBuilder.Append(operatingSystem);
        
        switch (platformId)
        {
            // Windows NT 6.1; Win64;
            case PlatformID.Win32NT:
            {
                osStringBuilder.Append(' ');
                
                osStringBuilder.Append(
                    RandomVersion(
                        minMajor:1, 
                        maxMajor:10, 
                        useSubVersion:true, 
                        minSubVersion:1, 
                        maxSubVersion:9));
                
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
                
                osStringBuilder.Append(
                    RandomVersion(
                        minMajor:1, 
                        maxMajor:17, 
                        minSubVersion:1, 
                        maxSubVersion:9, 
                        minMinor:1, 
                        maxMinor:9, 
                        useSubVersion:true, 
                        separator:'_'));
                
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

    private static StringBuilder StringBuilderByVersioning(
        int minMajor = 40,
        int maxMajor = 121,
        int minSubVersion = 40,
        int maxSubVersion = 121,
        int minMinor = 0,
        int maxMinor = 99,
        int minPatch = 0,
        int maxPatch = 99)
    {
        int digitsMinMajor = GetDigits(minMajor);
        int digitsMaxMajor = GetDigits(maxMajor);
        int digitsMinSubVersion = GetDigits(minSubVersion);
        int digitsMaxSubVersion = GetDigits(maxSubVersion);
        int digitsMinMinor = GetDigits(minMinor);
        int digitsMaxMinor = GetDigits(maxMinor);
        int digitsMinPatch = GetDigits(minPatch);
        int digitsMaxPatch = GetDigits(maxPatch);

        int sumDigits = digitsMinMajor + digitsMaxMajor + digitsMinSubVersion + 
                        digitsMaxSubVersion + digitsMinMinor +
                        digitsMaxMinor + digitsMinPatch + digitsMaxPatch;

        return new StringBuilder(sumDigits, sumDigits);
    }
    
    private static StringBuilder StringBuilderFromPlatform(PlatformID platformId)
    {
        switch (platformId)
        {
            case PlatformID.Win32NT:
                return new StringBuilder(22, 23);
            case PlatformID.MacOSX:
                return new StringBuilder(33, 33);
            case PlatformID.Unix:
                return new StringBuilder(15, 17);
            default:
                return new StringBuilder();
        }
    }

    public static Random Random
    {
        get => _random;
    }
}