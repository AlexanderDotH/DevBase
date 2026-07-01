namespace DevBase.Api.Apis.Musixmatch;

public sealed class MusixMatchEmptyTrackDataException : InvalidOperationException
{
    public MusixMatchEmptyTrackDataException(int trackId)
        : base($"Musixmatch track.get returned no track data for track id {trackId}.")
    {
        this.TrackId = trackId;
    }

    public int TrackId { get; }
}
