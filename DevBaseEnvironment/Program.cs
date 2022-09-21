using System.Diagnostics;
using DevBase.Generic;
using DevBaseApi.Apis.Tidal;
using DevBaseApi.Apis.Tidal.Structure.Json;

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
            TidalClient client = new TidalClient();
            JsonTidalSession session = client.Login(
                "eyJraWQiOiJ2OU1GbFhqWSIsImFsZyI6IkVTMjU2In0.eyJ0eXBlIjoibzJfYWNjZXNzIiwidWlkIjoxODczOTI3ODYsInNjb3BlIjoid19zdWIgcl91c3Igd191c3IiLCJnVmVyIjowLCJzVmVyIjowLCJjaWQiOjMyMzUsImV4cCI6MTY1OTYyODkyNSwic2lkIjoiNmM1YmJlZjEtODgwZi00M2Q3LTg5MjctMGQyODUzMGMxYjAzIiwiaXNzIjoiaHR0cHM6Ly9hdXRoLnRpZGFsLmNvbS92MSJ9.9Nbx5ImVvbxMDfgjXYk6Brf-7ZUw8iCmRjwAkP5W4aQ8oJlKYQPz5d-ehMXGuJckW7rV_Gs93d42pC8051_aIg").GetAwaiter().GetResult();

            JsonTidalSearchResult result = client.Search(session, "Never Gonna give you up").GetAwaiter().GetResult();
            Debug.WriteLine(result.TotalNumberOfItems);
        }

        
    }
}