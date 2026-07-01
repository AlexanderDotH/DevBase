using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.Net.Core;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    public List<string> ChartTracksGet(PaginationParameters? pagination = null)
    {
        return ChartTracksGetAsync(pagination).GetAwaiter().GetResult();
    }

    public async Task<List<string>> ChartTracksGetAsync(
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters resolvedPagination = ResolvePagination(pagination);
            Dictionary<string, string?> parameters = CreatePaginationParameters(resolvedPagination);

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.ChartTracksGet,
                parameters,
                cancellationToken: cancellationToken);
            
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            ChartTracksGet response = await this.ParseMusixmatchBodyAsync<ChartTracksGet>(
                rawResponse,
                cancellationToken);

            return (response?.TrackList ?? [])
                .Select(item => item.Track)
                .Where(track => !string.IsNullOrWhiteSpace(track))
                .ToList();
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<string>>(exception);
        }
    }

    public List<Artist> GetChartArtists(string country = "", PaginationParameters? pagination = null)
    {
        return GetChartArtistsAsync(country, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Artist>> GetChartArtistsAsync(
        string country = "",
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters resolvedPagination = ResolvePagination(pagination);
            Dictionary<string, string?> parameters = CreatePaginationParameters(resolvedPagination);
            parameters["country"] = country;

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.ChartArtistsGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            ChartArtistsGet response = await this.ParseMusixmatchBodyAsync<ChartArtistsGet>(
                rawResponse,
                cancellationToken);

            return (response?.Results ?? [])
                .Select(item => item.Artist)
                .Where(artist => artist is not null)
                .ToList()!;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Artist>>(exception);
        }
    }

    public List<Artist> ArtistSearch(string query, PaginationParameters? pagination = null)
    {
        return ArtistSearchAsync(query, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Artist>> ArtistSearchAsync(
        string query,
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(query);

            PaginationParameters resolvedPagination = ResolvePagination(pagination);
            Dictionary<string, string?> parameters = CreatePaginationParameters(resolvedPagination);
            parameters["q_artist"] = query;

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.ArtistSearch,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            ArtistSearch response = await this.ParseMusixmatchBodyAsync<ArtistSearch>(
                rawResponse,
                cancellationToken);

            return (response?.Results ?? [])
                .Select(item => item.Artist)
                .Where(artist => artist is not null)
                .ToList()!;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Artist>>(exception);
        }
    }

    public Artist GetArtistById(int id)
    {
        return GetArtistByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Artist> GetArtistByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        ArtistGet? response;

        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["artist_id"] = id.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.ArtistGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            response = await this.ParseMusixmatchBodyAsync<ArtistGet>(
                rawResponse,
                cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<Artist>(exception);
        }

        return RequireArtistResponse(response, id);
    }

    public List<Album> GetArtistAlbums(int id, PaginationParameters? pagination = null)
    {
        return GetArtistAlbumsAsync(id, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Album>> GetArtistAlbumsAsync(
        int id,
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters resolvedPagination = ResolvePagination(pagination);
            Dictionary<string, string?> parameters = CreatePaginationParameters(resolvedPagination);
            parameters["artist_id"] = id.ToString();

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.ArtistAlbumsGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            ArtistAlbumsGet response = await this.ParseMusixmatchBodyAsync<ArtistAlbumsGet>(
                rawResponse,
                cancellationToken);

            return (response?.Results ?? [])
                .Select(item => item.Album)
                .Where(album => album is not null)
                .ToList()!;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Album>>(exception);
        }
    }

    public Album GetAlbumById(int id)
    {
        return GetAlbumByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Album> GetAlbumByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        AlbumGet? response;

        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["album_id"] = id.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.AlbumGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            response = await this.ParseMusixmatchBodyAsync<AlbumGet>(
                rawResponse,
                cancellationToken);
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<Album>(exception);
        }

        return RequireAlbumResponse(response, id);
    }

    public List<Track> GetAlbumTracks(int id, PaginationParameters? pagination = null)
    {
        return GetAlbumTracksAsync(id, pagination).GetAwaiter().GetResult();
    }

    public async Task<List<Track>> GetAlbumTracksAsync(
        int id,
        PaginationParameters? pagination = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            PaginationParameters resolvedPagination = ResolvePagination(pagination);
            Dictionary<string, string?> parameters = CreatePaginationParameters(resolvedPagination);
            parameters["album_id"] = id.ToString();

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.AlbumTracksGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            AlbumTracksGet response = await this.ParseMusixmatchBodyAsync<AlbumTracksGet>(
                rawResponse,
                cancellationToken);

            return (response?.Results ?? [])
                .Select(item => item.Track)
                .Where(track => track is not null)
                .ToList()!;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Track>>(exception);
        }
    }

    public List<MusicGenre> GetMusicGenres()
    {
        return GetMusicGenresAsync().GetAwaiter().GetResult();
    }

    public async Task<List<MusicGenre>> GetMusicGenresAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> parameters = new();

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.MusicGenresGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response rawResponse = await request.SendAsync(cancellationToken);
            MusicGenresGet response = await this.ParseMusixmatchBodyAsync<MusicGenresGet>(
                rawResponse,
                cancellationToken);

            return (response?.Results ?? [])
                .Select(item => item.MusicGenre)
                .Where(genre => genre is not null)
                .ToList()!;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<MusicGenre>>(exception);
        }
    }

    private static PaginationParameters ResolvePagination(PaginationParameters? pagination)
    {
        return pagination ?? new PaginationParameters();
    }

    private static Dictionary<string, string?> CreatePaginationParameters(PaginationParameters pagination)
    {
        return new Dictionary<string, string?>
        {
            ["page"] = pagination.Page.ToString(),
            ["page_size"] = pagination.PageSize.ToString()
        };
    }

    private static Artist RequireArtistResponse(ArtistGet? response, int id)
    {
        return RequireResponseData(
            response?.Artist,
            "artist",
            id.ToString(),
            HasArtistData);
    }

    private static Album RequireAlbumResponse(AlbumGet? response, int id)
    {
        return RequireResponseData(
            response?.Album,
            "album",
            id.ToString(),
            HasAlbumData);
    }

    private static bool HasArtistData(Artist artist)
    {
        return artist.ArtistId > 0
               || !string.IsNullOrWhiteSpace(artist.ArtistName)
               || !string.IsNullOrWhiteSpace(artist.ArtistMbid)
               || !string.IsNullOrWhiteSpace(artist.ArtistCountry);
    }

    private static bool HasAlbumData(Album album)
    {
        return album.AlbumId > 0
               || album.ArtistId > 0
               || !string.IsNullOrWhiteSpace(album.AlbumName)
               || !string.IsNullOrWhiteSpace(album.AlbumMbid)
               || !string.IsNullOrWhiteSpace(album.ArtistName);
    }
}
