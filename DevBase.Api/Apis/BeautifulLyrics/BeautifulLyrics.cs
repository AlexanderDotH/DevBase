using System.Net;
using System.Text;
using DevBase.Api.Apis.BeautifulLyrics.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Api.Serializer;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.ResponseData;
using Newtonsoft.Json.Linq;

namespace DevBase.Api.Apis.BeautifulLyrics;

public class BeautifulLyrics : ApiClient
{
    private readonly string _baseUrl;

    public BeautifulLyrics()
    {
        this._baseUrl = "https://beautiful-lyrics.socalifornian.live";
    }

    public async Task<dynamic> GetLyrics(string isrc)
    {
        (string RawLyric, bool IsRichSync) rawLyrics = await this.GetRawLyrics(isrc);

        if (rawLyrics.IsRichSync)
        {
            return ParseRichSyncStampedLyrics(rawLyrics.RawLyric);
        }
        else
        {
            return ParseTimeStampedLyrics(rawLyrics.RawLyric);
        }
    }

    public async Task<(string RawLyrics, bool IsRichSync)> GetRawLyrics(string isrc)
    {
        string url = $"{this._baseUrl}/lyrics/{isrc}";

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        if (responseData.StatusCode != HttpStatusCode.OK)
            return Throw<object>(new BeautifulLyricsException(EnumBeautifulLyricsExceptionType.LyricsNotFound));
        
        string rawData = responseData.GetContentAsString();

        if (string.IsNullOrEmpty(rawData))
            return Throw<object>(new BeautifulLyricsException(EnumBeautifulLyricsExceptionType.LyricsNotFound));
        
        JObject responseObject = JObject.Parse(rawData);
        bool isRichSync = responseObject.Value<string>("Type") == "Syllable";
        
        return (rawData, isRichSync);
    }
    
    private AList<TimeStampedLyric> ParseTimeStampedLyrics(string rawLyrics)
    {
        JsonBeautifulLyricsLineLyricsResponse timeStampedLyricsResponse =
            new JsonDeserializer<JsonBeautifulLyricsLineLyricsResponse>().Deserialize(rawLyrics);

        AList<TimeStampedLyric> timeStampedLyrics = new AList<TimeStampedLyric>();

        if (timeStampedLyricsResponse == null)
            return Throw<object>(new BeautifulLyricsException(EnumBeautifulLyricsExceptionType.LyricsParsed));
        
        if (timeStampedLyricsResponse.VocalGroups == null || timeStampedLyricsResponse.VocalGroups.Count == 0)
            return Throw<object>(new BeautifulLyricsException(EnumBeautifulLyricsExceptionType.LyricsParsed));
        
        for (var i = 0; i < timeStampedLyricsResponse.VocalGroups.Count; i++)
        {
            JsonBeautifulLyricsLineLyricsResponseVocalGroup vocal = timeStampedLyricsResponse.VocalGroups[i];
            
            TimeSpan lineStartTime = TimeSpan.FromSeconds(vocal.StartTime);

            TimeStampedLyric timeStampedLyric = new TimeStampedLyric()
            {
                StartTime = lineStartTime,
                Text = vocal.Text
            };
            
            timeStampedLyrics.Add(timeStampedLyric);
        }

        return timeStampedLyrics;
    }
    
    // TODO: Fix parser for rich and line lyrics
    // TODO: Fix unit tests
    private AList<RichTimeStampedLyric> ParseRichSyncStampedLyrics(string rawLyrics)
    {
        JsonBeautifulLyricsRichLyricsResponse richLyricsResponse =
            new JsonDeserializer<JsonBeautifulLyricsRichLyricsResponse>().Deserialize(rawLyrics);

        AList<RichTimeStampedLyric> richLyrics = new AList<RichTimeStampedLyric>();

        if (richLyricsResponse == null)
            return Throw<object>(new BeautifulLyricsException(EnumBeautifulLyricsExceptionType.LyricsParsed));
        
        if (richLyricsResponse.VocalGroups == null || richLyricsResponse.VocalGroups.Count == 0)
            return Throw<object>(new BeautifulLyricsException(EnumBeautifulLyricsExceptionType.LyricsParsed));
        
        for (int i = 0; i < richLyricsResponse.VocalGroups.Count; i++)
        {
            JsonBeautifulLyricsRichLyricsResponseVocalGroups vocal = richLyricsResponse.VocalGroups[i];

            if (vocal.Lead == null || vocal.Lead.Count == 0)
                continue;
            
            TimeSpan lineStartTime = TimeSpan.FromSeconds(vocal.StartTime);
            TimeSpan lineEndTime = TimeSpan.FromSeconds(vocal.StartTime);

            StringBuilder lineTextContent = new StringBuilder();

            RichTimeStampedLyric timeStampedLyric = new RichTimeStampedLyric();
            
            for (var j = 0; j < vocal.Lead.Count; j++)
            {
                JsonBeautifulLyricsRichLyricsResponseVocalGroupsLead lead = vocal.Lead[j];

                TimeSpan wordStartTime = TimeSpan.FromSeconds(lead.StartTime);
                TimeSpan wordEndTime = TimeSpan.FromSeconds(lead.EndTime);

                string wordContent = lead.Text;
                
                RichTimeStampedWord timeStampedWord = new RichTimeStampedWord()
                {
                    Word = wordContent,
                    StartTime = wordStartTime,
                    EndTime = wordEndTime
                };

                lineTextContent.Append(wordContent + (j != vocal.Lead.Count - 1 ? " " : ""));
                
                timeStampedLyric.Words.Add(timeStampedWord);
            }

            timeStampedLyric.Text = lineTextContent.ToString();
            timeStampedLyric.StartTime = lineStartTime;
            timeStampedLyric.EndTime = lineEndTime;
            
            richLyrics.Add(timeStampedLyric);
        }

        return richLyrics;
    }
}