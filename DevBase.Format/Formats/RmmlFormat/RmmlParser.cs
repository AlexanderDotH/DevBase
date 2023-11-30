using System.Text;
using DevBase.Format.Formats.RmmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBase.Format.Formats.RmmlFormat;

public class RmmlParser : IFileFormat<AList<RichLyrics>>
{
    public AList<RichLyrics> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }
    
    public AList<RichLyrics> FormatFromString(string lyricString)
    {
        RichSyncLine[] parsedLyrics = JsonConvert.DeserializeObject<RichSyncLine[]>(lyricString);
        
        if (parsedLyrics == null)
            return default;

        AList<RichLyrics> richLyrics = new AList<RichLyrics>();

        for (var i = 0; i < parsedLyrics.Length; i++)
        {
            RichSyncLine r = parsedLyrics[i];
            
            TimeSpan rStartTime = TimeSpan.FromSeconds(r.TimeStart);
            TimeSpan rEndTime = TimeSpan.FromSeconds(r.TimeEnd);
            
            RichLyrics element = new RichLyrics()
            {
                Start = Convert.ToInt64(rStartTime.TotalMilliseconds),
                End = Convert.ToInt64(rEndTime.TotalMilliseconds),
                FullLine = r.FullLine,
                Elements = new List<RichLyricsElement>()
            };
            
            if (r.SingleCharOffsets != null && r.SingleCharOffsets.Count != 0)
            {
                TimeSpan lastOffset = TimeSpan.Zero;
                
                for (var j = 0; j < r.SingleCharOffsets.Count; j++)
                {
                    RichSyncChar l = r.SingleCharOffsets[j];

                    TimeSpan lOffset = TimeSpan.FromSeconds(l.Offset);
                    
                    TimeSpan lStartTime = rStartTime + lastOffset;
                    TimeSpan lEndTime = rStartTime + lOffset;
                    
                    RichLyricsElement syncChar = new RichLyricsElement()
                    {
                        Line = l.Char,
                        Start = Convert.ToInt64(lStartTime.TotalMilliseconds),
                        End = Convert.ToInt64(lEndTime.TotalMilliseconds)
                    };
                    
                    element.Elements.Add(syncChar);
                    lastOffset = lOffset;
                }
            }
            
            richLyrics.Add(element);
        }
        
        return richLyrics;
    }

    public string FormatToString(AList<RichLyrics> content)
    {
        throw new NotSupportedException("Not supported yet, it will be implemented if necessary");
    }
}