namespace DevBase.Api.Apis.Deezer.Structure.Objects;

public class DeezerTrack
{
    public string Title { get; set; }
    public string Album { get; set; }
    public int Duration { get; set; }
    public string[] Artists { get; set; }
    
    public string[] ArtworkUrls { get; set; }
    public string ServiceInternalId { get; set; }
}