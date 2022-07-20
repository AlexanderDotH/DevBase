using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.IO;
using DevBaseFormat.Formats.MmlFormat.Json;
using DevBaseFormat.Structure;
using DevBaseFormat.Utilities;
using Newtonsoft.Json;

namespace DevBaseFormat.Formats.MmlFormat
{
    public class MmlParser<T> : IFileFormat<T>
    {
        public T FormatFromFile(string filePath)
        {
            AFileObject file = AFile.ReadFile(filePath);
            return FormatFromString(file.ToStringData());
        }

        public T FormatFromString(string lyricString)
        {
            if (typeof(T) == typeof(LrcObject))
            {
                GenericList<LyricElement> lyricElements = new GenericList<LyricElement>();

                MmlElement[] parsedElements = JsonConvert.DeserializeObject<MmlElement[]>(lyricString);

                if (parsedElements == null)
                    return default;

                for (int i = 0; i < parsedElements.Length; i++)
                {
                    MmlElement mmlElement = parsedElements[i];

                    if (mmlElement == null)
                        continue;

                    long timeStamp = 0;
                    timeStamp += (long)TimeSpan.FromMinutes(mmlElement.Time.Minutes).TotalMilliseconds;
                    timeStamp += (long)TimeSpan.FromSeconds(mmlElement.Time.Seconds).TotalMilliseconds;
                    timeStamp += (long)TimeSpan.FromMilliseconds(mmlElement.Time.Hundredths).TotalMilliseconds;

                    lyricElements.Add(new LyricElement(
                        timeStamp, 
                        LyricsUtils.EditLine(mmlElement.Text)));
                }

                LrcObject lrcObject = new LrcObject();
                lrcObject.Lyrics = lyricElements;
                return (T)(object)lrcObject;
            }
            else
            {
                throw new Exception("Type is not supported use the \"LrcObject\" type");
            }
        }
    }
}
