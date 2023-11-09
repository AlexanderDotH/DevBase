using System.Globalization;
using CsvHelper;
using DevBase.Generics;
using DevBaseLive.Objects;
using DevBaseLive.Tracks;
using Newtonsoft.Json;

namespace DevBaseLive
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            TrackMiner miner = new TrackMiner(1000);

            List<Track> aTracks = (await miner.FindTracks()).GetAsList();

            File.WriteAllText("Tracks.json", JsonConvert.SerializeObject(aTracks));
            
            Console.WriteLine($"Wrote {aTracks.Count} tracks!");
        }
    }
}