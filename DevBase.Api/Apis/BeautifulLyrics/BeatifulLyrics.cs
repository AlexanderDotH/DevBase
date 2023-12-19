using System.Net;
using DevBase.Api.Apis.BeautifulLyrics.Structure.Json;
using DevBase.Api.Serializer;
using DevBase.Format.Formats.AppleLrcXmlFormat;
using DevBase.Format.Formats.AppleRichXmlFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Web;
using DevBase.Web.ResponseData;

namespace DevBase.Api.Apis.BeautifulLyrics;

public class BeatifulLyrics
{
    private readonly string _baseUrl;

    public BeatifulLyrics()
    {
        this._baseUrl = "https://beautiful-lyrics.socalifornian.live";
    }

    public async Task<dynamic> GetLyrics(string isrc)
    {
        JsonBeatifulLyricsLyricsResponse rawLyrics = await this.GetRawLyrics(isrc);

        if (rawLyrics == null)
            return default;
        
        if (rawLyrics.Content.Contains("Word"))
        {
            AList<RichTimeStampedLyric> richTimeStampedLyric = new AppleRichXmlParser().Parse(rawLyrics.Content);
            return richTimeStampedLyric;
        } 
        else if (rawLyrics.Content.Contains("Line"))
        {
            AList<TimeStampedLyric> timeStampedLyrics = new AppleLrcXmlParser().Parse(rawLyrics.Content);
            return timeStampedLyrics;
        }

        return default;
    }
    
    public async Task<JsonBeatifulLyricsLyricsResponse> GetRawLyrics(string isrc)
    {
        string url = $"{this._baseUrl}/lyrics/{isrc}";

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        if (responseData.StatusCode != HttpStatusCode.OK)
            return null;
        
        string rawData = responseData.GetContentAsString();

        if (string.IsNullOrEmpty(rawData))
            return null;
        
        return new JsonDeserializer<JsonBeatifulLyricsLyricsResponse>().Deserialize(rawData);
    }
}