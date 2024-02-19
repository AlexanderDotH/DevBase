using System.Diagnostics;
using System.Text;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Objects;
using DevBase.Requests.Preparation.Header.Body;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Body;

public class RequestFormHeaderBuilderTest
{
    [Test]
    public void RequestFormHeaderBuildTest()
    {
        int count = 1_000_000;
        
        var lorem = new Bogus.DataSets.Lorem("en");

        string[] keys = Enumerable.Range(0, count).Select(s => lorem.Word()).ToArray();
        
        string[] textValues = Enumerable.Range(0, count).Select(s => lorem.Word()).ToArray();
        byte[][] fileValues = Enumerable.Range(0, count).Select(s => Encoding.UTF8.GetBytes(lorem.Word())).ToArray();

        Stopwatch stopwatch = new Stopwatch();

        RequestFormHeaderBuilder lastHeader = null;
        MimeFileObject mimeFileObject = MimeFileObject.FromBuffer(fileValues[0]);
        
        stopwatch.Start();
        
        for (var i = 0; i < keys.Length; i++)
        {
            lastHeader = new RequestFormHeaderBuilder()
                .AddFile(keys[i], mimeFileObject)
                .AddText(keys[i], textValues[i])
                .Build();
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"Builded {count} form headers. Last entry: \n{lastHeader}\n");
        
        stopwatch.PrintTimeTable();
    }
}