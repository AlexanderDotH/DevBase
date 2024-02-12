using System.Diagnostics;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Preparation.Header.Body.Mime;

namespace DevBase.Test.DevBaseRequests.Preparation.Body.Mime;

public class MimeDictionaryTest
{
    [Test]
    public void GetMimeTypeTest()
    {
        int amount = 1_000_000;

        MimeDictionary dictionary = new MimeDictionary();

        ReadOnlySpan<char> mimeType = string.Empty;
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        
        for (int i = 0; i < amount; i++)
            mimeType = dictionary.GetMimeTypeAsSpan(".png");
        
        stopwatch.Stop();
        
        Console.WriteLine($"Got the mime type {amount}times for .png({mimeType.ToString()})");
        Console.WriteLine(stopwatch.GetTimeTable());
        
        Assert.AreEqual("image/png", mimeType.ToString());
    }
}