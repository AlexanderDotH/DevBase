using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.Generics;
using DevBase.Net.Core;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    public List<Track> SongSearch(
        TrackSearchParameters parameters,
        PaginationParameters? pagination = null)
    {
        return this.SongSearchAsync(parameters, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Track>> SongSearchAsync(
        TrackSearchParameters parameters,
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        try
        {
            PaginationParameters page = pagination ?? new PaginationParameters();
            KeyValuePair<string, string> sort = parameters.GetSortParameter();

            Dictionary<string, string?> requestParameters = new()
            {
                ["q"] = parameters.Query,
                ["q_lyrics"] = parameters.LyricsQuery,
                ["q_artist"] = parameters.Artist,
                ["q_track"] = parameters.Title,
                ["q_album"] = parameters.Album,
                ["f_has_lyrics"] = parameters.HasLyrics ? "1" : string.Empty,
                ["f_has_subtitle"] = parameters.HasSubtitles ? "1" : string.Empty,
                ["f_has_rich_sync"] = parameters.HasRichSync ? "1" : string.Empty,
                [sort.Key] = sort.Value,
                ["f_lyrics_language"] = parameters.Language,
                ["page"] = page.Page.ToString(),
                ["page_size"] = page.PageSize.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSearch,
                requestParameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);

            TrackSearch response = await this.ParseMusixmatchBodyAsync<TrackSearch>(rawResponse, cancellationToken);
            return ToTrackList(response);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return Throw<List<Track>>(exception);
        }
    }

    public List<Track> SongSearch(string query, PaginationParameters? pagination = null)
    {
        return this.SongSearchAsync(query, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Track>> SongSearchAsync(
        string query,
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters page = pagination ?? new PaginationParameters();

            Dictionary<string, string?> requestParameters = new()
            {
                ["q"] = query,
                ["page"] = page.Page.ToString(),
                ["page_size"] = page.PageSize.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSearch,
                requestParameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);

            TrackSearch response = await this.ParseMusixmatchBodyAsync<TrackSearch>(rawResponse, cancellationToken);
            return ToTrackList(response);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return Throw<List<Track>>(exception);
        }
    }

    public List<Track> SongSearch(
        string artist,
        string song,
        PaginationParameters? pagination = null)
    {
        return this.SongSearchAsync(artist, song, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Track>> SongSearchAsync(
        string artist,
        string song,
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters page = pagination ?? new PaginationParameters();

            Dictionary<string, string?> requestParameters = new()
            {
                ["q_artist"] = artist,
                ["q_track"] = song,
                ["page"] = page.Page.ToString(),
                ["page_size"] = page.PageSize.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSearch,
                requestParameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);

            TrackSearch response = await this.ParseMusixmatchBodyAsync<TrackSearch>(rawResponse, cancellationToken);
            return ToTrackList(response);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return Throw<List<Track>>(exception);
        }
    }

    public List<Track> SongSearchByLyrics(string lyrics)
    {
        return this.SongSearchByLyricsAsync(lyrics).GetAwaiter().GetResult();
    }

    public async Task<List<Track>> SongSearchByLyricsAsync(
        string lyrics,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> requestParameters = new()
            {
                ["q_lyrics"] = lyrics
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSearch,
                requestParameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);

            TrackSearch response = await this.ParseMusixmatchBodyAsync<TrackSearch>(rawResponse, cancellationToken);
            return ToTrackList(response);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return Throw<List<Track>>(exception);
        }
    }

    public Track GetTrackById(int id)
    {
        return this.GetTrackByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Track> GetTrackByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        TrackGet? response;

        try
        {
            Dictionary<string, string?> requestParameters = new()
            {
                ["track_id"] = id.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackGet,
                requestParameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);

            response = await this.ParseMusixmatchBodyAsync<TrackGet>(rawResponse, cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return Throw<Track>(exception);
        }

        return RequireTrackByIdResponse(response, id);
    }

    public string GetTrackSnippet(int id)
    {
        return this.GetTrackSnippetAsync(id).GetAwaiter().GetResult();
    }

    public async Task<string> GetTrackSnippetAsync(int id, CancellationToken cancellationToken = default)
    {
        TrackSnippetGet? response;

        try
        {
            Dictionary<string, string?> requestParameters = new()
            {
                ["track_id"] = id.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSnippetGet,
                requestParameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            response = await this.ParseMusixmatchBodyAsync<TrackSnippetGet>(
                rawResponse,
                cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return ThrowString(exception);
        }

        return RequireTrackSnippetResponse(response, id);
    }

    public Lyrics GetTrackLyrics(int id)
    {
        return this.GetTrackLyricsAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Lyrics> GetTrackLyricsAsync(int id, CancellationToken cancellationToken = default)
    {
        TrackLyricsGet? response;

        try
        {
            Dictionary<string, string?> requestParameters = new()
            {
                ["track_id"] = id.ToString(),
                ["part"] = "user,lyrics_verified_by"
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackLyricsGet,
                requestParameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            response = await this.ParseMusixmatchBodyAsync<TrackLyricsGet>(
                rawResponse,
                cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return Throw<Lyrics>(exception);
        }

        return RequireTrackLyricsResponse(response, id);
    }

    private async Task<T> ParseMusixmatchBodyAsync<T>(
        Response response,
        CancellationToken cancellationToken)
    {
        string content = await response.GetStringAsync(cancellationToken: cancellationToken);
        MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);

        if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success) {
            return this.Throw<T>(new MusixMatchRequestException(apiResponse.StatusCode));
        }

        return apiResponse.Cast<T>()!;
    }

    private static List<Track> ToTrackList(TrackSearch response)
    {
        return new GenericTypeConversion<TrackSearch.TrackList, Track>()
            .MergeToList(
                response.Results ?? [],
                (result, tracks) =>
                {
                    if (result.Track is not null) {
                        tracks.Add(result.Track);
                    }
                })
            .ToList();
    }

    private static Track RequireTrackByIdResponse(TrackGet? response, int id)
    {
        Track? track = response?.Track;

        if (track is null || !HasTrackData(track))
        {
            throw new MusixMatchEmptyTrackDataException(id);
        }

        return track;
    }

    private static string RequireTrackSnippetResponse(TrackSnippetGet? response, int id)
    {
        Snippet? snippet = response?.Snippet;

        if (snippet is null) {
            throw new MusixMatchEmptyResponseDataException("snippet", id.ToString());
        }

        if (snippet.Instrumental != 0) {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(snippet.SnippetBody)) {
            throw new MusixMatchEmptyResponseDataException("snippet", id.ToString());
        }

        return snippet.SnippetBody;
    }

    private static Lyrics RequireTrackLyricsResponse(TrackLyricsGet? response, int id)
    {
        Lyrics? lyrics = response?.Lyrics;

        return RequireResponseData(
            lyrics,
            "lyrics",
            id.ToString(),
            HasLyricsData);
    }

    private static bool HasTrackData(Track track)
    {
        return track.TrackId.HasValue
               || track.CommontrackId.HasValue
               || !string.IsNullOrWhiteSpace(track.TrackMbid)
               || !string.IsNullOrWhiteSpace(track.TrackIsrc)
               || !string.IsNullOrWhiteSpace(track.TrackSpotifyId)
               || !string.IsNullOrWhiteSpace(track.TrackName)
               || !string.IsNullOrWhiteSpace(track.ArtistName)
               || !string.IsNullOrWhiteSpace(track.AlbumName);
    }

    private static bool HasLyricsData(Lyrics lyrics)
    {
        return lyrics.LyricsId.HasValue
               || lyrics.Instrumental.HasValue
               || !string.IsNullOrWhiteSpace(lyrics.LyricsBody)
               || !string.IsNullOrWhiteSpace(lyrics.LyricsLanguage)
               || !string.IsNullOrWhiteSpace(lyrics.LyricsCopyright);
    }
}
