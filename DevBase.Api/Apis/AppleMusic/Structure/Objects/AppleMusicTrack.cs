using DevBase.Api.Apis.AppleMusic.Structure.Json;
using DevBase.Generics;

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

    public static AppleMusicTrack FromResponse(JsonAppleMusicSearchResultResultsSongData response)
    {
        AppleMusicTrack appleMusicTrack = new AppleMusicTrack()
        {
            Title = response.Attributes.Name,
            Album = response.Attributes.AlbumName,
            Duration = response.Attributes.DurationInMillis,
            Artists = GetArtists(response.Relationships.Artists),
            ArtworkUrls = GetArtworkUrls(response.Attributes.Artwork),
            ServiceInternalId = response.Id,
            Isrc = response.Attributes.Isrc
        };

        return appleMusicTrack;
    }

    private static string[] GetArtworkUrls(JsonAppleMusicSearchResultResultsSongDataAttributesArtwork artwork)
    {
        string artworkUrl = artwork.Url;
        artworkUrl = artworkUrl.Replace("{w}", artwork.Width.ToString());
        artworkUrl = artworkUrl.Replace("{h}", artwork.Height.ToString());

        return new string[] { artworkUrl };
    }
    
    private static string[] GetArtists(JsonAppleMusicSearchResultResultsSongDataRelationshipsArtists artistResponse)
    {
        AList<string> artists = new AList<string>();

        for (var i = 0; i < artistResponse.Data.Count; i++)
        {
            JsonAppleMusicSearchResultResultsSongData songData = artistResponse.Data[i];
            
            if (songData.Attributes == null)
                continue;
            
            if (string.IsNullOrEmpty(songData.Attributes.ArtistName))
                continue;
            
            artists.Add(songData.Attributes.ArtistName);
        }

        return artists.GetAsArray();
    }
}