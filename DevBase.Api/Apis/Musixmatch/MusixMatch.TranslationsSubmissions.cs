using DevBase.Api.Apis.Musixmatch.Internal;
using DevBase.Api.Apis.Musixmatch.Structure.Requests;
using DevBase.Api.Apis.Musixmatch.Structure.Types;
using DevBase.Net.Core;
using Newtonsoft.Json;
using System.Text;

namespace DevBase.Api.Apis.Musixmatch;

public partial class MusixMatch
{
    public string GetLyricsTranslation(int id, string language)
    {
        return this.GetLyricsTranslationAsync(id, language).GetAwaiter().GetResult();
    }

    public async Task<string> GetLyricsTranslationAsync(
        int id,
        string language,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Lyrics lyrics = await this.GetTrackLyricsAsync(id, cancellationToken);
            string translatedLyrics = lyrics.LyricsBody ?? string.Empty;

            List<CrowdTrackTranslationsGet.Translation> translations =
                await this.GetLyricsTranslationsRawAsync(id, language, cancellationToken);

            foreach (CrowdTrackTranslationsGet.Translation translation in translations)
            {
                if (!string.IsNullOrEmpty(translation.Snippet)) {
                    translatedLyrics = translatedLyrics.Replace(translation.Snippet, translation.Description);
                }
            }

            return translatedLyrics;
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<string>(exception);
        }
    }

    public List<Translation> GetLyricsTranslationLines(int id, string language)
    {
        return this.GetLyricsTranslationLinesAsync(id, language).GetAwaiter().GetResult();
    }

    public async Task<List<Translation>> GetLyricsTranslationLinesAsync(
        int id,
        string language,
        CancellationToken cancellationToken = default)
    {
        try
        {
            List<CrowdTrackTranslationsGet.Translation> rawTranslations =
                await this.GetLyricsTranslationsRawAsync(id, language, cancellationToken);

            return rawTranslations
                .Select(translation => new Translation
                {
                    OriginalLine = translation.Snippet,
                    TranslatedLine = translation.Description
                })
                .ToList();
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<Translation>>(exception);
        }
    }

    public List<CrowdTrackTranslationsGet.Translation> GetLyricsTranslationsRaw(int id, string language)
    {
        return this.GetLyricsTranslationsRawAsync(id, language).GetAwaiter().GetResult();
    }

    public async Task<List<CrowdTrackTranslationsGet.Translation>> GetLyricsTranslationsRawAsync(
        int id,
        string language,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string?> parameters = new()
            {
                ["track_id"] = id.ToString(),
                ["selected_language"] = language
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.CrowdTrackTranslationsGet,
                parameters,
                cancellationToken: cancellationToken);

            Request request = new Request(url);
            Response httpResponse = await request.SendAsync(cancellationToken);

            string content = await httpResponse.GetStringAsync(cancellationToken: cancellationToken);
            MusixMatchApiResponse apiResponse = MusixMatchResponseParser.Parse(content, this.StrictErrorHandling);

            if (apiResponse.StatusCode != (int)MusixMatchStatusCode.Success) {
                return this.Throw<List<CrowdTrackTranslationsGet.Translation>>(
                    new MusixMatchRequestException(apiResponse.StatusCode));
            }

            CrowdTrackTranslationsGet response = apiResponse.Cast<CrowdTrackTranslationsGet>()!;

            return response.TranslationsList
                .Select(root => root.Translation)
                .ToList();
        }
        catch (System.Exception exception) when (exception is not OperationCanceledException)
        {
            return this.Throw<List<CrowdTrackTranslationsGet.Translation>>(exception);
        }
    }

    /// <summary>
    /// Submits lyrics to Musixmatch for the selected track and may change account/community state.
    /// </summary>
    public void SubmitTrackLyrics(int id, string lyrics)
    {
        this.SubmitTrackLyricsAsync(id, lyrics).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Submits lyrics to Musixmatch for the selected track and may change account/community state.
    /// </summary>
    public async Task SubmitTrackLyricsAsync(
        int id,
        string lyrics,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Track track = await this.GetTrackByIdAsync(id, cancellationToken);

            Dictionary<string, string?> parameters = new()
            {
                ["commontrack_id"] = track.CommontrackId.ToString(),
                ["q_track"] = track.TrackName,
                ["q_artist"] = track.ArtistName
            };

            Dictionary<string, string?> data = new()
            {
                ["lyrics_body"] = lyrics
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackLyricsPost,
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
            _ = this.Throw<object>(exception);
        }
    }

    /// <summary>
    /// Submits raw translation lines to Musixmatch and may change account/community state.
    /// </summary>
    public void SubmitTrackTranslationsRaw(
        int id,
        List<TranslationPost> translation,
        int timeSpent = int.MaxValue)
    {
        this.SubmitTrackTranslationsRawAsync(id, translation, timeSpent).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Submits raw translation lines to Musixmatch and may change account/community state.
    /// </summary>
    public async Task SubmitTrackTranslationsRawAsync(
        int id,
        List<TranslationPost> translation,
        int timeSpent = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(translation);

            Track track = await this.GetTrackByIdAsync(id, cancellationToken);

            Dictionary<string, string?> parameters = new()
            {
                ["commontrack_id"] = track.CommontrackId.ToString(),
                ["track_id"] = track.TrackId.ToString()
            };

            Dictionary<string, string?> data = new()
            {
                ["translations_list"] = JsonConvert.SerializeObject(translation),
                ["time_spent"] = timeSpent.ToString()
            };

            string url = await this.BuildMusixMatchUrlAsync(
                MusixMatchEndpoint.TrackLyricsTranslationPost,
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
            _ = this.Throw<object>(exception);
        }
    }
}
