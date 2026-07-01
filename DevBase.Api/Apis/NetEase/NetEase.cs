using DevBase.Api.Apis.NetEase.Structure.Json;
using DevBase.Api.Enums;
using DevBase.Api.Exceptions;
using DevBase.Format;
using DevBase.Format.Formats.KLyricsFormat;
using DevBase.Format.Formats.LrcFormat;
using DevBase.Format.Structure;
using DevBase.Generics;
using DevBase.Net.Core;
using DevBase.Utilities;

namespace DevBase.Api.Apis.NetEase;

public class NetEase : ApiClient
{
    private readonly string _baseUrl;

    public NetEase()
    {
        this._baseUrl = "https://music.xianqiao.wang/neteaseapiv2";
    }

    public Task<JsonNetEaseDetailResponse> TrackDetails(params string[] trackIds)
    {
        return this.TrackDetails(trackIds.AsEnumerable());
    }

    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseDetailResponse> TrackDetails(
        IEnumerable<string> trackIds,
        CancellationToken cancellationToken = default)
    {
        string separated = StringUtils.Separate(trackIds.ToArray(), ",");

        Request request = new Request($"{this._baseUrl}/song/detail?ids={separated}")
            .AsGet()
            .WithTimeout(TimeSpan.FromMinutes(1));

        Response response = await request.SendAsync(cancellationToken);
        return await response.ParseJsonAsync<JsonNetEaseDetailResponse>(false, cancellationToken);
    }
    #pragma warning restore SYSLIB0013

    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseSearchResult> Search(
        string keyword,
        int limit = 50,
        int type = 1,
        CancellationToken cancellationToken = default)
    {
        Request request = new Request($"{this._baseUrl}/search")
            .AsGet()
            .WithParameters(
                ("limit", limit.ToString()),
                ("type", type.ToString()),
                ("keywords", keyword)
            );

        Response response = await request.SendAsync(cancellationToken);
        return await response.ParseJsonAsync<JsonNetEaseSearchResult>(false, cancellationToken);
    }
    #pragma warning restore SYSLIB0013
    
    public async Task<JsonNetEaseDetailResponse> SearchAndReceiveDetails(
        string keyword,
        int limit = 50,
        int type = 1,
        CancellationToken cancellationToken = default)
    {
        JsonNetEaseSearchResult searchResult = await Search(keyword, limit, type, cancellationToken);

        if (searchResult == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        }

        if (searchResult.result == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        }

        if (searchResult.result.songs == null || searchResult.result.songs.Count == 0) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        }

        AList<string> trackIds = new AList<string>();
        
        for (var i = 0; i < searchResult.result.songs.Count; i++)
        {
            trackIds.Add(Convert.ToString(searchResult.result.songs[i].id));
        }

        if (trackIds.IsEmpty()) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptySearchResults));
        }

        return await TrackDetails(trackIds.GetAsArray(), cancellationToken);
    }
    
    #pragma warning disable S1751
    public async Task<byte[]> Download(string trackId, CancellationToken cancellationToken = default)
    {
        JsonNetEaseUrlResponse urlResponse = await Url(trackId, cancellationToken);

        if (urlResponse == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyUrls));
        }

        if (urlResponse.data == null || urlResponse.data.Count == 0) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyUrls));
        }

        for (var i = 0; i < urlResponse.data.Count; i++)
        {
            JsonNetEaseUrlResponseData data = urlResponse.data[i];

            Request request = new Request(data.url).AsGet();
            Response response = await request.SendAsync(cancellationToken);
            return await response.GetBytesAsync(cancellationToken);
        }

        return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.DownloadTrack));
    }
    #pragma warning restore S1751
    
    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseUrlResponse> Url(string trackId, CancellationToken cancellationToken = default)
    {
        Request request = new Request($"{this._baseUrl}/song/url")
            .AsGet()
            .WithParameter("id", trackId);

        Response response = await request.SendAsync(cancellationToken);
        return await response.ParseJsonAsync<JsonNetEaseUrlResponse>(false, cancellationToken);
    }
    #pragma warning restore SYSLIB0013
    
    public async Task<AList<RichTimeStampedLyric>> KaraokeLyrics(
        string trackId,
        CancellationToken cancellationToken = default)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId, cancellationToken);

        if (lyricResponse == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        }

        if (lyricResponse.klyric == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        }

        if (String.IsNullOrEmpty(lyricResponse.klyric.lyric)) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        }

        AList<RichTimeStampedLyric> richLyrics = new KLyricsParser().Parse(lyricResponse.klyric.lyric);
        return richLyrics;
    }
    
    public async Task<AList<TimeStampedLyric>> Lyrics(
        string trackId,
        CancellationToken cancellationToken = default)
    {
        JsonNetEaseLyricResponse lyricResponse = await this.RawLyrics(trackId, cancellationToken);

        if (lyricResponse == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        }

        if (lyricResponse.lrc == null) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        }

        if (String.IsNullOrEmpty(lyricResponse.lrc.lyric)) {
            return Throw<object>(new NetEaseException(EnumNetEaseExceptionType.EmptyLyrics));
        }

        AList<TimeStampedLyric> lyrics = new LrcParser().Parse(lyricResponse.lrc.lyric);
        return lyrics;
    }
    
    #pragma warning disable SYSLIB0013
    public async Task<JsonNetEaseLyricResponse> RawLyrics(
        string trackId,
        CancellationToken cancellationToken = default)
    {
        Request request = new Request($"{this._baseUrl}/lyric")
            .AsGet()
            .WithParameter("id", trackId)
            .WithTimeout(TimeSpan.FromMinutes(1));

        Response response = await request.SendAsync(cancellationToken);
        return await response.ParseJsonAsync<JsonNetEaseLyricResponse>(false, cancellationToken);
    }
    #pragma warning restore SYSLIB0013
}
