using DevBase.Api.Apis.NetEase.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
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

public class NetEase : ApiClient
{
    private readonly string _baseUrl;

    public NetEase()
    {
        this._baseUrl = "https://music.xianqiao.wang/neteaseapiv2";
    }

    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseDetailResponse> TrackDetails(params string[] trackIds)
    {
        string separated = StringUtils.Separate(trackIds, ",");
        
        string url = Uri.EscapeUriString($"{this._baseUrl}/song/detail?ids={separated}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        return new JsonDeserializer<JsonNetEaseDetailResponse>().Deserialize(content);
    }
    #pragma warning restore SYSLIB0013

    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseSearchResult> Search(string keyword, int limit = 50, int type = 1)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/search?limit={limit}&type={type}&keywords={keyword}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        
        return new JsonDeserializer<JsonNetEaseSearchResult>().Deserialize(content);
    }
    #pragma warning restore SYSLIB0013
    
    public async Task<JsonNetEaseDetailResponse> SearchAndReceiveDetails(string keyword, int limit = 50, int type = 1)
    {
        JsonNetEaseSearchResult searchResult = await Search(keyword, limit, type);

        if (searchResult == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        
        if (searchResult.result == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));

        if (searchResult.result.songs == null || searchResult.result.songs.Count == 0)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        
        AList<string> trackIds = new AList<string>();
        
        for (var i = 0; i < searchResult.result.songs.Count; i++)
            trackIds.Add(Convert.ToString(searchResult.result.songs[i].id));

        if (trackIds.IsEmpty())
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        
        return await TrackDetails(trackIds.GetAsArray());
    }
    
    #pragma warning disable S1751
    public async Task<byte[]> Download(string trackId)
    {
        JsonNetEaseUrlResponse urlResponse = await Url(trackId);

        if (urlResponse == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyUrls));

        if (urlResponse.data == null || urlResponse.data.Count == 0)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyUrls));
        
        for (var i = 0; i < urlResponse.data.Count; i++)
        {
            JsonNetEaseUrlResponseData data = urlResponse.data[i];

            Request request = new Request(data.url);
            ResponseData responseData = await request.GetResponseAsync();
            
            return responseData.Content;
        }

        return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.DownloadTrack));
    }
    #pragma warning restore S1751
    
    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseUrlResponse> Url(string trackId)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/song/url?id={trackId}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        
        return new JsonDeserializer<JsonNetEaseUrlResponse>().Deserialize(content);
    }
    #pragma warning restore SYSLIB0013
    
    public async Task<AList<RichTimeStampedLyric>> KaraokeLyrics(string trackId)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId);

        if (lyricResponse == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));

        if (lyricResponse.klyric == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));

        if (String.IsNullOrEmpty(lyricResponse.klyric.lyric))
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));

        AList<RichTimeStampedLyric> richLyrics = new KLyricsParser().Parse(lyricResponse.klyric.lyric);
        return richLyrics;
    }
    
    public async Task<AList<TimeStampedLyric>> Lyrics(string trackId)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId);

        if (lyricResponse == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));

        if (lyricResponse.lrc == null)
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));

        if (String.IsNullOrEmpty(lyricResponse.lrc.lyric))
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        
        AList<TimeStampedLyric> lyrics = new LrcParser().Parse(lyricResponse.lrc.lyric);
        return lyrics;
    }
    
    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseLyricResponse> RawLyrics(string trackId)
    {
        string url = Uri.EscapeUriString($"{this._baseUrl}/lyric?id={trackId}");

        Request request = new Request(url);
        ResponseData responseData = await request.GetResponseAsync();

        string content = responseData.GetContentAsString();
        return new JsonDeserializer<JsonNetEaseLyricResponse>().Deserialize(content);
    }
    #pragma warning restore SYSLIB0013
}