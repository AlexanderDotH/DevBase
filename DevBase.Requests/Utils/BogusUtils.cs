using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DevBase.Requests.Utils;

public class BogusUtils
{
    private static (string identifier, bool hasSub) _plattforms;

    static BogusUtils()
    {
        _plattforms = new[]
        {
            "Windows NT",
            "Linux; Android",
            "X11; Linux x86_64"
        };
    }
    
    public static string RandomOperatingSystemPlattform()
    {
        
    }
}