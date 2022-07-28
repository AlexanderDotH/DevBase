using DevBase.Generic;

namespace DevBaseEnvironment
{
    public class Track
    {
        public string Name { get; set; }
    }

    public class Results
    {
        public List<TrackList> Tracks { get; set; }

        public class TrackList
        {
            public Track Track;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {

            Results results = new Results();
            
            Track t1 = new Track();
            t1.Name = "SUMMER OF MY LIFE";

            Track t2 = new Track();
            t2.Name = "Summer";

            results.Tracks = new List<Results.TrackList>();

            Results.TrackList results1 = new Results.TrackList();
            results1.Track = t1;

            Results.TrackList results2 = new Results.TrackList();
            results2.Track = t2;

            results.Tracks.Add(results1);
            results.Tracks.Add(results2);

            List<Track> merge = new GenericTypeConversion<Results.TrackList, Track>().MergeToList(results.Tracks, (input,tracks) => tracks.Add(input.Track));

            foreach (Track t in merge)
            {
                Console.WriteLine(t.Name);
            }

        }
    }
}