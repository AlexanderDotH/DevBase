using DevBase.Api.Apis.NetEase.Structure.Json;
using DevBase.Api.Serializer;
using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
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

    public async Task<JsonNetEaseSearchResult> Search(string keyword, int limit = 50, int type = 1)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/search?limit={limit}&type={type}&keywords={keyword}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        return new JsonDeserializer<JsonNetEaseSearchResult>().Deserialize(content);
    }
    
    public async Task<AList<RichLyrics>> KaraokeLyrics(string trackId)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId);

        if (lyricResponse == null)
            return null;

        if (lyricResponse.klyric == null)
            return null;

        if (String.IsNullOrEmpty(lyricResponse.klyric.lyric))
            return null;
        
        FileFormatParser<AList<RichLyrics>> klrcParser =
            new FileFormatParser<AList<RichLyrics>>(
                new KLyricsParser());

        AList<RichLyrics> richLyrics = klrcParser.FormatFromString(lyricResponse.klyric.lyric);
        return richLyrics;
    }
    
    public async Task<AList<LyricElement>> Lyrics(string trackId)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId);

        if (lyricResponse == null)
            return null;

        if (lyricResponse.lrc == null)
            return null;

        if (String.IsNullOrEmpty(lyricResponse.lrc.lyric))
            return null;
        
        FileFormatParser<LrcObject> lrcParser =
            new FileFormatParser<LrcObject>(
                new LrcParser<LrcObject>());

        LrcObject lrcObject = lrcParser.FormatFromString(lyricResponse.lrc.lyric);
        return lrcObject.Lyrics;
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