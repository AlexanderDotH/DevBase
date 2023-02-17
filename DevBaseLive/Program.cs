using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.RequestData.Types;
using DevBase.Api.Apis.Deezer;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Generics;
using Newtonsoft.Json;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {
            Deezer deezerAPi = new Deezer();
            
            Console.Write("Enter a search term: ");

            string searchTerm = Console.ReadLine()!;
            
            Console.WriteLine("----------------------------------------------");
                
            var searchResults = deezerAPi.Search(searchTerm).GetAwaiter().GetResult();

            if (searchResults.total == 0)
                Console.WriteLine("No songs found!");
                
            ATupleList<int, JsonDeezerSearchDataResponse> results =
                new ATupleList<int, JsonDeezerSearchDataResponse>();

            for (int i = 0; i < searchResults.data.Count; i++)
            {
                var result = searchResults.data[i];
                    
                results.Add(i, result);

                Console.WriteLine("{0} : {1}({2}) from {3}", i, result.title, result.album.title, result.artist.name);
            }
            
            Console.WriteLine("----------------------------------------------");
            
            Console.Write("Choose one number: ");

            string input = Console.ReadLine();
                
            if (!(Char.IsNumber(input, 0) && input.Length == 1))
                return;

            JsonDeezerSearchDataResponse selected = results.FindEntry(Convert.ToInt32(input));

            var data = deezerAPi.DownloadSong(selected.id.ToString()).GetAwaiter().GetResult();
            File.WriteAllBytes(string.Format("{0}.mp3", selected.title), data);
            
            Console.WriteLine(string.Format("Song saved as {0}.mp3", selected.title));
        }
    }
}