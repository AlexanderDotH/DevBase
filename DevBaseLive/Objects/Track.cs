namespace DevBaseLive.Objects;

/// <summary>
/// Represents a music track with basic metadata.
/// </summary>
public class Track
{
    /// <summary>
    /// Gets or sets the title of the track.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the album name.
    /// </summary>
    public string Album { get; set; }
    
    /// <summary>
    /// Gets or sets the duration of the track in seconds (or milliseconds, depending on source).
    /// </summary>
    public int Duration { get; set; }
    
    /// <summary>
    /// Gets or sets the list of artists associated with the track.
    /// </summary>
    public string[] Artists { get; set; }
}