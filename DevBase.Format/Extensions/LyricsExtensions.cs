using System.Text;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Utilities;

namespace DevBase.Format.Extensions;

public static class LyricsExtensions
{
    public static string ToPlainText(this AList<RawLyric> rawElements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < rawElements.Length; i++)
            rawLyrics.AppendLine(rawElements.Get(i).Text);

        return rawLyrics.ToString();
    }
    
    public static string ToPlainText(this AList<TimeStampedLyric> elements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < elements.Length; i++)
            rawLyrics.AppendLine(elements.Get(i).Text);

        return rawLyrics.ToString();
    }
    
    public static string ToPlainText(this AList<RichTimeStampedLyric> richElements)
    {
        StringBuilder rawLyrics = new StringBuilder();

        for (int i = 0; i < richElements.Length; i++)
            rawLyrics.AppendLine(richElements.Get(i).Text);

        return rawLyrics.ToString();
    }
    
    public static AList<TimeStampedLyric> ToTimeStampedLyrics(this AList<RichTimeStampedLyric> richElements)
    {
        AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

        for (int i = 0; i < richElements.Length; i++)
        {
            RichTimeStampedLyric stampedLyric = richElements.Get(i);

            TimeStampedLyric timeStampedLyric = new TimeStampedLyric()
            {
                Text = stampedLyric.Text,
                StartTime = stampedLyric.StartTime
            };
            
            timeStampedLyrics.Add(timeStampedLyric);
        }
        
        return timeStampedLyrics;
    }
}