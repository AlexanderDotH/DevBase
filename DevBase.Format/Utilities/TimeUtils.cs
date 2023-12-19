﻿using System.Globalization;

namespace DevBase.Format.Utilities;

public class TimeUtils
{
    // Dude don't ask me why its 10pm and its too late for me but if you can fix that go ahead!
    private static readonly string[] _formats = new[]
    {
        "h\\:mm\\:ss",
        "hh\\:m\\:ss",
        "hh\\:mm\\:s",
        "h\\:mm\\:ss\\.fff",
        "hh\\:m\\:ss\\.fff",
        "hh\\:mm\\:s\\.fff",
        "h\\:mm\\:ss\\.ff",
        "hh\\:m\\:ss\\.ff",
        "hh\\:mm\\:s\\.ff",
        "h\\:mm\\:ss\\.f",
        "hh\\:m\\:ss\\.f",
        "hh\\:mm\\:s\\.f",
        "mm\\:ss",
        "m\\:ss",
        "mm\\:s",
        "mm\\:ss\\.fff",
        "m\\:ss\\.fff",
        "mm\\:s\\.fff",
        "mm\\:ss\\.ff",
        "m\\:ss\\.ff",
        "mm\\:s\\.ff",
        "mm\\:ss\\.f",
        "m\\:ss\\.f",
        "mm\\:s\\.f",
        "ss\\.fff",
        "s\\.fff"
    };

    public static bool TryParseTimeStamp(string time, out TimeSpan timeSpan)
    {
        for (int i = 0; i < _formats.Length; i++)
        {
            if (TimeSpan.TryParseExact(time, _formats[i], null, TimeSpanStyles.None, out timeSpan))
                return true;

            if (TimeSpan.TryParse(time, out timeSpan))
                return true;
        }

        timeSpan = TimeSpan.MinValue;
        
        return false;
    }
    
    public static TimeSpan ParseTimeStamp(string time)
    {
        TimeSpan timeSpan = TimeSpan.Zero;

        if (TryParseTimeStamp(time, out timeSpan))
            return timeSpan;
       
        throw new System.Exception("Cannot format timestamp");
    }
}