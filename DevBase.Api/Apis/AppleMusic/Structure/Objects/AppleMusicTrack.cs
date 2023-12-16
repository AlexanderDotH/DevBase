namespace DevBase.Api.Apis.AppleMusic.Structure.Objects;

public class AppleMusicTrack
{
    public string Title { get; set; }
    public string Album { get; set; }
    public int Duration { get; set; }
    public string[] Artists { get; set; }
    
    public string[] ArtworkUrls { get; set; }
    public string ServiceInternalId { get; set; }
    public string Isrc { get; set; }
}