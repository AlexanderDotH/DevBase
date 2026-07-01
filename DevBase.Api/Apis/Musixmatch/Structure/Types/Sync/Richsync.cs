using Newtonsoft.Json;

namespace DevBase.Api.Apis.Musixmatch.Structure.Types;

public sealed class Richsync
{
    public Richsync()
    {
    }

    public Richsync(string? serializedRichsync)
    {
        if (string.IsNullOrWhiteSpace(serializedRichsync)) {
            return;
        }

        List<MusixmatchRichsyncFormattedLine>? lines =
            JsonConvert.DeserializeObject<List<MusixmatchRichsyncFormattedLine>>(serializedRichsync);

        this.LoadFromMusixmatchRichsyncLines(lines);
    }

    public Richsync(List<MusixmatchRichsyncFormattedLine>? lines)
    {
        this.LoadFromMusixmatchRichsyncLines(lines);
    }

    public List<RichsyncLine> Lines { get; set; } = [];

    public List<RichsyncCharacter> Characters { get; set; } = [];

    public List<MusixmatchRichsyncFormattedLine> ToMusixmatchRichsync()
    {
        List<MusixmatchRichsyncFormattedLine> encodedLines = [];

        foreach (RichsyncLine line in this.Lines)
        {
            List<MusixmatchRichsyncFormattedLine.SingleCharOffset> charOffsets = [];

            foreach (RichsyncCharacter character in line.Characters)
            {
                charOffsets.Add(new MusixmatchRichsyncFormattedLine.SingleCharOffset
                {
                    Char = character.Character,
                    Offset = character.TimeOffset.TotalSeconds
                });
            }

            encodedLines.Add(new MusixmatchRichsyncFormattedLine
            {
                FullLine = line.FullLine,
                TimeStart = line.TimeStart.TotalSeconds,
                TimeEnd = line.TimeEnd.TotalSeconds,
                SingleCharOffsets = charOffsets
            });
        }

        return encodedLines;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this.ToMusixmatchRichsync());
    }

    private void LoadFromMusixmatchRichsyncLines(List<MusixmatchRichsyncFormattedLine>? lines)
    {
        this.Lines = [];
        this.Characters = [];

        if (lines is null) {
            return;
        }

        foreach (MusixmatchRichsyncFormattedLine line in lines)
        {
            this.AddRichsyncLine(line);
        }
    }

    private void AddRichsyncLine(MusixmatchRichsyncFormattedLine source)
    {
        RichsyncLine line = new()
        {
            FullLine = source.FullLine,
            TimeStart = TimeSpan.FromSeconds(source.TimeStart),
            TimeEnd = TimeSpan.FromSeconds(source.TimeEnd),
            Characters = []
        };

        foreach (MusixmatchRichsyncFormattedLine.SingleCharOffset sourceCharacter in source.SingleCharOffsets)
        {
            RichsyncCharacter character = new()
            {
                Character = sourceCharacter.Char,
                TimeOffset = TimeSpan.FromSeconds(sourceCharacter.Offset),
                TimeStart = TimeSpan.FromSeconds(sourceCharacter.Offset).Add(line.TimeStart)
            };

            line.Characters.Add(character);
            this.Characters.Add(character);
        }

        this.Lines.Add(line);
    }
}
