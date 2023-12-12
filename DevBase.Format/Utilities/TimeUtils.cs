using System.Globalization;

namespace DevBase.Format.Utilities;

public class TimeUtils
{
    public static TimeSpan ParseTimeStamp(string time)
    {
        TimeSpan parsed;

        string[] formats = new[] { "ss\\.fff", "m\\:ss\\.fff", "mm\\:ss\\.fff", "hh\\:mm\\:ss\\.fff" };

        for (int i = 0; i < formats.Length; i++)
        {
            if (TimeSpan.TryParseExact(time, formats[i], null, TimeSpanStyles.None, out parsed))
                return parsed;
        }

        throw new System.Exception("Cannot format timestamp");
    }

}