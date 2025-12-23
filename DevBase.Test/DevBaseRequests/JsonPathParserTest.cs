using System.Text;
using DevBase.Requests.Parsing;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class JsonPathParserTest
{
    private JsonPathParser _parser = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new JsonPathParser();
    }

    [Test]
    public void Parse_SimpleProperty_ReturnsValue()
    {
        var json = "{\"name\":\"John\"}"u8.ToArray();
        
        var result = _parser.Parse<string>(json, "$.name");
        
        Assert.That(result, Is.EqualTo("John"));
    }

    [Test]
    public void Parse_NestedProperty_ReturnsValue()
    {
        var json = "{\"user\":{\"name\":\"John\",\"age\":30}}"u8.ToArray();
        
        var result = _parser.Parse<string>(json, "$.user.name");
        
        Assert.That(result, Is.EqualTo("John"));
    }

    [Test]
    public void Parse_ArrayIndex_ReturnsValue()
    {
        var json = "{\"items\":[\"a\",\"b\",\"c\"]}"u8.ToArray();
        
        var result = _parser.Parse<string>(json, "$.items[1]");
        
        Assert.That(result, Is.EqualTo("b"));
    }

    [Test]
    public void Parse_NestedObject_ReturnsObject()
    {
        var json = "{\"data\":{\"id\":1,\"value\":\"test\"}}"u8.ToArray();
        
        var result = _parser.Parse<TestData>(json, "$.data");
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Value, Is.EqualTo("test"));
    }

    [Test]
    public void ParseList_ArrayWildcard_ReturnsAllItems()
    {
        var json = "{\"items\":[{\"id\":1},{\"id\":2},{\"id\":3}]}"u8.ToArray();
        
        var result = _parser.ParseList<TestItem>(json, "$.items[*]");
        
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[1].Id, Is.EqualTo(2));
        Assert.That(result[2].Id, Is.EqualTo(3));
    }

    [Test]
    public void Parse_NonExistentPath_ReturnsDefault()
    {
        var json = "{\"name\":\"John\"}"u8.ToArray();
        
        var result = _parser.Parse<string>(json, "$.nonexistent");
        
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Parse_NumberValue_ReturnsNumber()
    {
        var json = "{\"count\":42}"u8.ToArray();
        
        var result = _parser.Parse<int>(json, "$.count");
        
        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Parse_BooleanValue_ReturnsBoolean()
    {
        var json = "{\"active\":true}"u8.ToArray();
        
        var result = _parser.Parse<bool>(json, "$.active");
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void Parse_DeeplyNested_ReturnsValue()
    {
        var json = "{\"level1\":{\"level2\":{\"level3\":{\"value\":\"deep\"}}}}"u8.ToArray();
        
        var result = _parser.Parse<string>(json, "$.level1.level2.level3.value");
        
        Assert.That(result, Is.EqualTo("deep"));
    }

    private class TestData
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("value")]
        public string Value { get; set; } = "";
    }

    private class TestItem
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
