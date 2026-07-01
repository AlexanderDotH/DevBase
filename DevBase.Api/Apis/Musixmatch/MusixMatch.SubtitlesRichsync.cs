using System.Globalization;
using System.Text;
using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Api.Apis.Musixmatch.Enums;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.Net.Core;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    public SubtitleRawResponse GetTrackSubtitlesRaw(
        int id,
        MusixMatchSubtitleFormat format = MusixMatchSubtitleFormat.Lrc)
    {
        return this.GetTrackSubtitlesRawAsync(id, format).GetAwaiter().GetResult();
    }

    public async Task<SubtitleRawResponse> GetTrackSubtitlesRawAsync(
        int id,
        MusixMatchSubtitleFormat format = MusixMatchSubtitleFormat.Lrc,
        CancellationToken cancellationToken = default)
    {
        TrackSubtitleGet? response;

        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["track_id"] = id.ToString(CultureInfo.InvariantCulture),
                ["subtitle_format"] = ToSubtitleFormatParameter(format)
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSubtitleGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response httpResponse = await request.SendAsync(cancellationToken);
            string content = await httpResponse.GetStringAsync(cancellationToken: cancellationToken);
            MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);

            if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success)
            {
                TrackSubtitleGet? failedResponse = this.Throw<TrackSubtitleGet>(
                    new MusixMatchRequestException(apiResponse.StatusCode));

                return RequireSubtitleResponse(failedResponse, id);
            }

            response = apiResponse.Cast<TrackSubtitleGet>();
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<SubtitleRawResponse>(exception);
        }

        return RequireSubtitleResponse(response, id);
    }

    public Subtitles GetTrackSubtitles(int id)
    {
        return this.GetTrackSubtitlesAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Subtitles> GetTrackSubtitlesAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        SubtitleRawResponse synced = await this.GetTrackSubtitlesRawAsync(
            id,
            MusixMatchSubtitleFormat.Musixmatch,
            cancellationToken);

        return new Subtitles(synced.SubtitleBody);
    }

    /// <summary>
    /// Submits raw subtitles to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackSubtitlesRaw(int id, string subtitles)
    {
        this.SubmitTrackSubtitlesRawAsync(id, subtitles).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Submits raw subtitles to Musixmatch and may change account/community state.
    /// </summary>
    public async Task SubmitTrackSubtitlesRawAsync(
        int id,
        string subtitles,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(subtitles);

            Track track = await this.GetTrackByIdAsync(id, cancellationToken);
            Dictionary<string, string?> parameters = CreateTrackSubmissionParameters(
                track,
                Random.Shared.Next(16, 2048),
                Random.Shared.Next(2048, 1048576));
            Dictionary<string, string?> data = new()
            {
                ["subtitle_body"] = subtitles
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackSubtitlePost,
                parameters,
                cancellationToken: cancellationToken);
            string body = BuildMusixMatchBody(data);
            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response httpResponse = await request.SendAsync(cancellationToken);
            string content = await httpResponse.GetStringAsync(cancellationToken: cancellationToken);
            MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);

            if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success) {
                this.Throw<object>(new MusixMatchRequestException(apiResponse.StatusCode));
            }
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            this.Throw<object>(exception);
        }
    }

    /// <summary>
    /// Submits subtitles to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackSubtitles(int id, Subtitles subtitles)
    {
        this.SubmitTrackSubtitlesRaw(id, subtitles.ToString());
    }

    /// <summary>
    /// Submits subtitles to Musixmatch and may change account/community state.
    /// </summary>
    public Task SubmitTrackSubtitlesAsync(
        int id,
        Subtitles subtitles,
        CancellationToken cancellationToken = default)
    {
        return this.SubmitTrackSubtitlesRawAsync(id, subtitles.ToString(), cancellationToken);
    }

    public RichsyncRawResponse GetTrackRichsyncRaw(int id)
    {
        return this.GetTrackRichsyncRawAsync(id).GetAwaiter().GetResult();
    }

    public async Task<RichsyncRawResponse> GetTrackRichsyncRawAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        TrackRichsyncGet? response;

        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["track_id"] = id.ToString(CultureInfo.InvariantCulture)
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackRichsyncGet,
                parameters,
                cancellationToken: cancellationToken);
            Request request = new Request(url);
            Response httpResponse = await request.SendAsync(cancellationToken);
            string content = await httpResponse.GetStringAsync(cancellationToken: cancellationToken);
            MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);

            if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success)
            {
                TrackRichsyncGet? failedResponse = this.Throw<TrackRichsyncGet>(
                    new MusixMatchRequestException(apiResponse.StatusCode));

                return RequireRichsyncResponse(failedResponse, id);
            }

            response = apiResponse.Cast<TrackRichsyncGet>();
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<RichsyncRawResponse>(exception);
        }

        return RequireRichsyncResponse(response, id);
    }

    public Richsync GetTrackRichsync(int id)
    {
        return this.GetTrackRichsyncAsync(id).GetAwaiter().GetResult();
    }

    public async Task<Richsync> GetTrackRichsyncAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        RichsyncRawResponse richsyncRawResponse = await this.GetTrackRichsyncRawAsync(
            id,
            cancellationToken);

        return new Richsync(richsyncRawResponse.RichsyncBody);
    }

    /// <summary>
    /// Submits raw richsync to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackRichsyncRaw(int id, string richsync)
    {
        this.SubmitTrackRichsyncRawAsync(id, richsync).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Submits raw richsync to Musixmatch and may change account/community state.
    /// </summary>
    public async Task SubmitTrackRichsyncRawAsync(
        int id,
        string richsync,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(richsync);

            Track track = await this.GetTrackByIdAsync(id, cancellationToken);
            Dictionary<string, string?> parameters = CreateTrackSubmissionParameters(
                track,
                Random.Shared.Next(32, 4086),
                Random.Shared.Next(2048, 1048576));
            Dictionary<string, string?> data = new()
            {
                ["richsync_body"] = richsync
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackRichsyncPost,
                parameters,
                cancellationToken: cancellationToken);
            string body = BuildMusixMatchBody(data);
            Request request = new Request(url)
                .AsPost()
                .WithRawBody(body, Encoding.UTF8)
                .WithHeader("Content-Type", GetMusixMatchContentType(body));
            Response httpResponse = await request.SendAsync(cancellationToken);
            string content = await httpResponse.GetStringAsync(cancellationToken: cancellationToken);
            MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);

            if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success) {
                this.Throw<object>(new MusixMatchRequestException(apiResponse.StatusCode));
            }
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            this.Throw<object>(exception);
        }
    }

    /// <summary>
    /// Submits richsync to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackRichsync(int id, Richsync richsync)
    {
        this.SubmitTrackRichsyncRaw(id, richsync.ToString());
    }

    /// <summary>
    /// Submits richsync to Musixmatch and may change account/community state.
    /// </summary>
    public Task SubmitTrackRichsyncAsync(
        int id,
        Richsync richsync,
        CancellationToken cancellationToken = default)
    {
        return this.SubmitTrackRichsyncRawAsync(id, richsync.ToString(), cancellationToken);
    }

    private static string ToSubtitleFormatParameter(MusixMatchSubtitleFormat format)
    {
        return format switch
        {
            MusixMatchSubtitleFormat.Lrc => "lrc",
            MusixMatchSubtitleFormat.Dfxp => "dfxp",
            MusixMatchSubtitleFormat.Stledu => "stledu",
            MusixMatchSubtitleFormat.Musixmatch => "mxm",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    private static Dictionary<string, string?> CreateTrackSubmissionParameters(
        Track track,
        int numKeypressed,
        int timeSpent)
    {
        return new Dictionary<string, string?>
        {
            ["commontrack_id"] = track.CommontrackId?.ToString(CultureInfo.InvariantCulture),
            ["length"] = track.TrackLength?.ToString(CultureInfo.InvariantCulture),
            ["q_track"] = track.TrackName,
            ["original_title"] = track.TrackName,
            ["q_artist"] = track.ArtistName,
            ["original_artist"] = track.ArtistName,
            ["original_uri"] = track.TrackSpotifyId,
            ["num_keypressed"] = numKeypressed.ToString(CultureInfo.InvariantCulture),
            ["time_spent"] = timeSpent.ToString(CultureInfo.InvariantCulture)
        };
    }

    private static SubtitleRawResponse RequireSubtitleResponse(TrackSubtitleGet? response, int id)
    {
        return RequireResponseData(
            response?.Subtitle,
            "subtitle",
            id.ToString(CultureInfo.InvariantCulture),
            HasSubtitleData);
    }

    private static RichsyncRawResponse RequireRichsyncResponse(TrackRichsyncGet? response, int id)
    {
        return RequireResponseData(
            response?.Richsync,
            "richsync",
            id.ToString(CultureInfo.InvariantCulture),
            HasRichsyncData);
    }

    private static bool HasSubtitleData(SubtitleRawResponse subtitle)
    {
        return subtitle.SubtitleId > 0
               || subtitle.SubtitleLength > 0
               || !string.IsNullOrWhiteSpace(subtitle.SubtitleBody)
               || !string.IsNullOrWhiteSpace(subtitle.SubtitleLanguage);
    }

    private static bool HasRichsyncData(RichsyncRawResponse richsync)
    {
        return richsync.RichsyncId > 0
               || richsync.RichsyncLength > 0
               || !string.IsNullOrWhiteSpace(richsync.RichsyncBody)
               || !string.IsNullOrWhiteSpace(richsync.RichsyncLanguage);
    }
}
