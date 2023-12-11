using System.Text;
using DevBase.Format.Formats.RmmlFormat.Json;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBase.Format.Formats.RmmlFormat;

public class RmmlParser : IFileFormat<AList<RichTimeStampedLyric>>
{
    public AList<RichTimeStampedLyric> FormatFromFile(string filePath)
    {
        AFileObject file = AFile.ReadFile(filePath);
        return FormatFromString(file.ToStringData());
    }
    
    public AList<RichTimeStampedLyric> FormatFromString(string lyricString)
    {
        RichSyncLine[] parsedLyrics = JsonConvert.DeserializeObject<RichSyncLine[]>(lyricString);
        
        if (parsedLyrics == null)
            return default;

        AList<RichTimeStampedLyric> richLyrics = new AList<RichTimeStampedLyric>();

        for (var i = 0; i < parsedLyrics.Length; i++)
        {
            RichSyncLine r = parsedLyrics[i];
            
            TimeSpan rStartTime = TimeSpan.FromSeconds(r.TimeStart);
            TimeSpan rEndTime = TimeSpan.FromSeconds(r.TimeEnd);
            
            RichTimeStampedLyric element = new RichTimeStampedLyric()
            {
                StartTime = rStartTime,
                EndTime = rEndTime,
                Text = r.FullLine,
                Words = new AList<RichTimeStampedWord>()
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
                    
                    RichTimeStampedWord richWord = new RichTimeStampedWord()
                    {
                        StartTime = lStartTime,
                        EndTime = lEndTime,
                        Word = l.Char
                    };
                    
                    element.Words.Add(richWord);
                    lastOffset = lOffset;
                }
            }
            
            richLyrics.Add(element);
        }
        
        return richLyrics;
    }

    public string FormatToString(AList<RichTimeStampedLyric> content)
    {
        throw new NotSupportedException("Not supported yet, it will be implemented if necessary");
    }
}