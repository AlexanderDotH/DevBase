using System.Diagnostics;
using System.Text;
using Bogus.DataSets;
using DevBase.Extensions.Stopwatch;
using DevBase.Requests.Abstract;
using DevBase.Requests.Data.Header.Body;
using DevBase.Requests.Data.Header.Body.Content;
using DevBase.Test.Test;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;

namespace DevBase.Test.DevBaseRequests.Preparation.Header.Body;

public class RequestRawBodyBuilderTest
{
    private const int _count = 1_000_000;
    private string _textValue;
    private string _jsonValue;
    private byte[] _bufferValue;
    
    [SetUp]
    public void GenerateDummyData()
    {
        Lorem lorem = new Lorem("en");
        this._textValue = lorem.Paragraph();

        JObject jsonObject = new JObject();
        jsonObject["firstname"] = lorem.Word();
        jsonObject["lastname"] = lorem.Word();
        jsonObject["age"] = new Random().Next(0, 100);

        this._jsonValue = jsonObject.ToString();

        this._bufferValue = Encoding.UTF8.GetBytes(this._textValue);
    }

    // With validation
    [Test]
    public void RequestRawBodyBuilderStringValidationTest() =>
        RequestRawBodyBuilderTestRunner(new StringRequestContent(Encoding.UTF8), true);

    [Test]
    public void RequestRawBodyBuilderJsonValidationTest() =>
        RequestRawBodyBuilderTestRunner(new JsonRequestContent(Encoding.UTF8), true);

    [Test]
    public void RequestRawBodyBuilderBufferValidationTest() =>
        RequestRawBodyBuilderTestRunner(new BufferRequestContent(), true);
    
    // Without validation
    [Test]
    public void RequestRawBodyBuilderStringTest() =>
        RequestRawBodyBuilderTestRunner(new StringRequestContent(Encoding.UTF8), false);

    [Test]
    public void RequestRawBodyBuilderJsonTest() =>
        RequestRawBodyBuilderTestRunner(new JsonRequestContent(Encoding.UTF8), false);

    [Test]
    public void RequestRawBodyBuilderBufferTest() =>
        RequestRawBodyBuilderTestRunner(new BufferRequestContent(), false);
    
    private void RequestRawBodyBuilderTestRunner<T>(T builder, bool useValidation) where T : RequestContent
    {
        Memory<byte> buffer = new Memory<byte>();

        // ψ(｀∇´)ψ
        Action penetrationAction = null;

        if (builder is StringRequestContent stringContent)
        {
            penetrationAction = () =>
            {
                RequestRawBodyBuilder rawBodyBuilder = new RequestRawBodyBuilder(useValidation)
                    .WithText(this._textValue, stringContent.Encoding)
                    .Build();

                buffer = rawBodyBuilder.Buffer;
            };
        }

        if (builder is JsonRequestContent jsonContent)
        {
            penetrationAction = () =>
            {
                RequestRawBodyBuilder rawBodyBuilder = new RequestRawBodyBuilder(useValidation)
                    .WithJson(this._jsonValue, jsonContent.Encoding)
                    .Build();

                buffer = rawBodyBuilder.Buffer;
            };
        }
        
        if (builder is BufferRequestContent)
        {
            penetrationAction = () =>
            {
                RequestRawBodyBuilder rawBodyBuilder = new RequestRawBodyBuilder(useValidation)
                    .WithBuffer(this._bufferValue)
                    .Build();

                buffer = rawBodyBuilder.Buffer;
            };
        }
        
        if (penetrationAction == null)
            throw new InvalidTestFixtureException("Wrong builder type provided");
        
        Stopwatch stopwatch = PenetrationTest.Run(penetrationAction, _count);
        
        Console.WriteLine($"Validation: {useValidation}\nBuilt {_count} raw form body. Last entry: \n{Encoding.UTF8.GetString(buffer.ToArray())}\n");
        
        stopwatch.PrintTimeTable();
        
        Assert.IsNotEmpty(buffer.ToArray());
    }
}