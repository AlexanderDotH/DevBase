using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.Body.Mime;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Body.Mime;

public class MimeDictionaryTest
{
    [Test]
    public void GetMimeTypeTest()
    {
        int amount = 1_000_000;

        MimeDictionary dictionary = new MimeDictionary();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        for (int i = 0; i < amount; i++)
            dictionary.GetMimeType(".png");
        
        stopwatch.Stop();
        
        Console.WriteLine($"Got the mime type {amount}times for .png({dictionary.GetMimeType(".png")})");
        Console.WriteLine(stopwatch.GetTimeTable());
    }
}