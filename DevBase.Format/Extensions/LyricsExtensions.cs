using System.Text;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Utilities;

namespace DevBase.Format.Extensions;

public static class LyricsExtensions
{
    public static string ToRaw(this AList<LyricElement> elements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < elements.Length; i++)
            rawLyrics.AppendLine(elements.Get(i).Line);

        return rawLyrics.ToString();
    }
    
    public static string ToRaw(this AList<RichLyrics> richElements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < richElements.Length; i++)
            rawLyrics.AppendLine(richElements.Get(i).FullLine);

        return rawLyrics.ToString();
    }
}