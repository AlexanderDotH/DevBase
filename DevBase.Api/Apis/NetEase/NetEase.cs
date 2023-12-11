using DevBase.Api.Apis.NetEase.Structure.Json;
using DevBase.Api.Serializer;
using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Utilities;
using DevBase.Web;
using DevBase.Web.ResponseData;

namespace DevBase.Api.Apis.NetEase;

public class NetEase
{
    private readonly string _baseUrl;

    public NetEase()
    {
        this._baseUrl = "https://music.xianqiao.wang/neteaseapiv2";
    }

    public async Task<JsonNetEaseDetailResponse> TrackDetails(params string[] trackIds)
    {
        string separated = StringUtils.Separate(trackIds, ",");
        
        string url = Uri.EscapeUriString($"{this._baseUrl}/song/detail?ids={separated}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        return new JsonDeserializer<JsonNetEaseDetailResponse>().Deserialize(content);
    }

    public async Task<JsonNetEaseSearchResult> Search(string keyword, int limit = 50, int type = 1)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/search?limit={limit}&type={type}&keywords={keyword}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        
        return new JsonDeserializer<JsonNetEaseSearchResult>().Deserialize(content);
    }
    
    public async Task<JsonNetEaseDetailResponse> SearchAndReceiveDetails(string keyword, int limit = 50, int type = 1)
    {
        JsonNetEaseSearchResult searchResult = await Search(keyword, limit, type);

        if (searchResult == null)
            return null;
        
        if (searchResult.result == null)
            return null;

        if (searchResult.result.songs == null || searchResult.result.songs.Count == 0)
            return null;
        
        AList<string> trackIds = new AList<string>();
        
        for (var i = 0; i < searchResult.result.songs.Count; i++)
            trackIds.Add(Convert.ToString(searchResult.result.songs[i].id));

        if (trackIds.IsEmpty())
            return null;
        
        return await TrackDetails(trackIds.GetAsArray());
    }
    
    public async Task<byte[]> Download(string trackId)
    {
        JsonNetEaseUrlResponse urlResponse = await Url(trackId);

        if (urlResponse == null)
            return null;

        if (urlResponse.data == null || urlResponse.data.Count == 0)
            return null;
        
        for (var i = 0; i < urlResponse.data.Count; i++)
        {
            JsonNetEaseUrlResponseData data = urlResponse.data[i];

            Request request = new Request(data.url);
            ResponseData responseData = await request.GetResponseAsync();
            return responseData.Content;
        }

        return null;
    }
    
    public async Task<JsonNetEaseUrlResponse> Url(string trackId)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/song/url?id={trackId}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        
        return new JsonDeserializer<JsonNetEaseUrlResponse>().Deserialize(content);
    }
    
    public async Task<AList<RichTimeStampedLyric>> KaraokeLyrics(string trackId)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId);

        if (lyricResponse == null)
            return null;

        if (lyricResponse.klyric == null)
            return null;

        if (String.IsNullOrEmpty(lyricResponse.klyric.lyric))
            return null;
        
        FileFormatParser<AList<RichTimeStampedLyric>> klrcParser =
            new FileFormatParser<AList<RichTimeStampedLyric>>(
                new KLyricsParser());

        AList<RichTimeStampedLyric> richLyrics = klrcParser.FormatFromString(lyricResponse.klyric.lyric);
        return richLyrics;
    }
    
    public async Task<AList<TimeStampedLyric>> Lyrics(string trackId)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId);

        if (lyricResponse == null)
            return null;

        if (lyricResponse.lrc == null)
            return null;

        if (String.IsNullOrEmpty(lyricResponse.lrc.lyric))
            return null;
        
        FileFormatParser<AList<TimeStampedLyric>> lrcParser =
            new FileFormatParser<AList<TimeStampedLyric>>(
                new LrcParser<AList<TimeStampedLyric>>());

        AList<TimeStampedLyric> lyrics = lrcParser.FormatFromString(lyricResponse.lrc.lyric);
        return lyrics;
    }
    
    public async Task<JsonNetEaseLyricResponse> RawLyrics(string trackId)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/lyric?id={trackId}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        return new JsonDeserializer<JsonNetEaseLyricResponse>().Deserialize(content);
    }
}