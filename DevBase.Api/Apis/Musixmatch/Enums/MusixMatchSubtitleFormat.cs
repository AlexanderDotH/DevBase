namespace DevBase.Api.Apis.Musixmatch.Enums;

/// <summary>
/// Represents a Musixmatch subtitle response format.
/// </summary>
public enum MusixMatchSubtitleFormat
{
    /// <summary>LRC subtitle format.</summary>
    Lrc,

    /// <summary>DFXP XML subtitle format.</summary>
    Dfxp,

    /// <summary>STLEDU subtitle format.</summary>
    Stledu,

    /// <summary>Musixmatch JSON subtitle format used by the desktop client.</summary>
    Musixmatch
}
