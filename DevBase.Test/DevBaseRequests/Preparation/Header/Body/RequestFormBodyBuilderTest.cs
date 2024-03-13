using System.Diagnostics;
using System.Text;
using Bogus.DataSets;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Objects;
using DevBase.Requests.Preparation.Header.Body;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Body;

public class RequestFormBodyBuilderTest
{

    private const int _count = 1_000_000;
    private string[] _keys;
    private string[] _textValues;
    private byte[][] _fileValues;
    
    [SetUp]
    public void GenerateDummyData()
    {
        Lorem lorem = new Lorem("en");
        _keys = Enumerable.Range(0, _count).Select(s => lorem.Word()).ToArray();
        _textValues = Enumerable.Range(0, _count).Select(s => lorem.Word()).ToArray();
        _fileValues = Enumerable.Range(0, _count).Select(s => Encoding.UTF8.GetBytes(lorem.Word())).ToArray();
    }
    
    [Test]
    public void RequestFormHeaderBuildTest()
    {
        Stopwatch stopwatch = new Stopwatch();

        RequestFormBodyBuilder lastBody = null;
        MimeFileObject mimeFileObject = MimeFileObject.FromBuffer(_fileValues[0]);
        
        stopwatch.Start();
        
        for (var i = 0; i < this._keys.Length; i++)
        {
            lastBody = new RequestFormBodyBuilder()
                .AddFile(this._keys[i], mimeFileObject)
                .AddText(this._keys[i], this._textValues[i])
                .Build();
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"Builded {_count} form headers. Last entry: \n{lastBody}\n");
        
        stopwatch.PrintTimeTable();
    }
}