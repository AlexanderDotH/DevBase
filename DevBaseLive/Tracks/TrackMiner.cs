using CrypticWizard.RandomWordGenerator;
using DevBase.Api.Apis.Tidal;
using DevBase.Api.Apis.Tidal.Structure.Json;
using DevBase.Generics;
using DevBaseLive.Objects;

namespace DevBaseLive.Tracks;

public class TrackMiner
{
    private string[] _searchParams;

    private Tidal _tidal;

    public TrackMiner(int searchParams)
    {
        this._searchParams = new WordGenerator()
            .GetWords(WordGenerator.PartOfSpeech.verb, searchParams).ToArray();

        this._tidal = new Tidal();
    }
    
    public async Task<AList<Track>> FindTracks()
    {
        AList<Track> tracks = new AList<Track>();
        
        for (var i = 0; i < this._searchParams.Length; i++)
        {
            JsonTidalSearchResult result = await this._tidal.Search(this._searchParams[i], limit:1000);
            
            Console.WriteLine($"Found {result.Items.Count} items for word {this._searchParams[i]!}");
            
            result.Items.ForEach(r =>
            {
                Track t = new Track()
                {
                    Title = r.Title,
                    Album = r.Album.Title,
                    Artists = ConvertArtists(r.Artists),
                    Duration = r.Duration
                };
                
                tracks.Add(t);
            });
        }

        return tracks;
    }
    
    private string[] ConvertArtists(List<JsonTidalArtist> artists)
    {
        List<string> artistList = new List<string>();
        
        artists.ForEach(a =>
        {
            artistList.Add(a.Name);
        });

        return artistList.ToArray();
    }
}