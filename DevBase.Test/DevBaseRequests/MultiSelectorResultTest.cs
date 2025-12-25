using System.Text;
using System.Text.Json;
using DevBase.Net.Parsing;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class MultiSelectorResultTest
{
    [Test]
    public void GetString_ExistingValue_ReturnsString()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": ""test""}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetString("key");

        Assert.That(value, Is.EqualTo("test"));
    }

    [Test]
    public void GetString_NonExistingValue_ReturnsNull()
    {
        var result = new MultiSelectorResult();

        var value = result.GetString("nonexistent");

        Assert.That(value, Is.Null);
    }

    [Test]
    public void GetInt_ExistingValue_ReturnsInt()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": 42}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetInt("key");

        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public void GetInt_NonExistingValue_ReturnsNull()
    {
        var result = new MultiSelectorResult();

        var value = result.GetInt("nonexistent");

        Assert.That(value, Is.Null);
    }

    [Test]
    public void GetLong_ExistingValue_ReturnsLong()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": 9223372036854775807}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetLong("key");

        Assert.That(value, Is.EqualTo(9223372036854775807L));
    }

    [Test]
    public void GetDouble_ExistingValue_ReturnsDouble()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": 123.45}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetDouble("key");

        Assert.That(value, Is.EqualTo(123.45).Within(0.001));
    }

    [Test]
    public void GetBool_TrueValue_ReturnsTrue()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": true}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetBool("key");

        Assert.That(value, Is.True);
    }

    [Test]
    public void GetBool_FalseValue_ReturnsFalse()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": false}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetBool("key");

        Assert.That(value, Is.False);
    }

    [Test]
    public void GetBool_NonBoolValue_ReturnsNull()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": ""notabool""}").RootElement.GetProperty("value");
        result.Set("key", element);

        var value = result.GetBool("key");

        Assert.That(value, Is.Null);
    }

    [Test]
    public void Get_ComplexObject_DeserializesCorrectly()
    {
        var result = new MultiSelectorResult();
        var json = @"{""user"": {""id"": 123, ""name"": ""John""}}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("user");
        result.Set("user", element);

        var user = result.Get<TestUser>("user");

        Assert.That(user, Is.Not.Null);
        Assert.That(user.Id, Is.EqualTo(123));
        Assert.That(user.Name, Is.EqualTo("John"));
    }

    [Test]
    public void HasValue_ExistingValue_ReturnsTrue()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""value"": ""test""}").RootElement.GetProperty("value");
        result.Set("key", element);

        Assert.That(result.HasValue("key"), Is.True);
    }

    [Test]
    public void HasValue_NonExistingValue_ReturnsFalse()
    {
        var result = new MultiSelectorResult();

        Assert.That(result.HasValue("nonexistent"), Is.False);
    }

    [Test]
    public void HasValue_NullValue_ReturnsFalse()
    {
        var result = new MultiSelectorResult();
        result.Set("key", null);

        Assert.That(result.HasValue("key"), Is.False);
    }

    [Test]
    public void Names_MultipleValues_ReturnsAllNames()
    {
        var result = new MultiSelectorResult();
        var doc = JsonDocument.Parse(@"{""a"": 1, ""b"": 2, ""c"": 3}");
        result.Set("first", doc.RootElement.GetProperty("a"));
        result.Set("second", doc.RootElement.GetProperty("b"));
        result.Set("third", doc.RootElement.GetProperty("c"));

        var names = result.Names.ToList();

        Assert.That(names, Contains.Item("first"));
        Assert.That(names, Contains.Item("second"));
        Assert.That(names, Contains.Item("third"));
        Assert.That(names.Count, Is.EqualTo(3));
    }

    [Test]
    public void Count_MultipleValues_ReturnsCorrectCount()
    {
        var result = new MultiSelectorResult();
        var doc = JsonDocument.Parse(@"{""a"": 1, ""b"": 2}");
        result.Set("first", doc.RootElement.GetProperty("a"));
        result.Set("second", doc.RootElement.GetProperty("b"));

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void Count_EmptyResult_ReturnsZero()
    {
        var result = new MultiSelectorResult();

        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetString_ObjectValue_ReturnsRawJson()
    {
        var result = new MultiSelectorResult();
        var element = JsonDocument.Parse(@"{""obj"": {""nested"": ""value""}}").RootElement.GetProperty("obj");
        result.Set("key", element);

        var value = result.GetString("key");

        Assert.That(value, Is.Not.Null);
        Assert.That(value, Does.Contain("nested"));
        Assert.That(value, Does.Contain("value"));
    }

    [Test]
    public void Set_OverwriteExisting_UpdatesValue()
    {
        var result = new MultiSelectorResult();
        var doc1 = JsonDocument.Parse(@"{""value"": 1}");
        var doc2 = JsonDocument.Parse(@"{""value"": 2}");
        
        result.Set("key", doc1.RootElement.GetProperty("value"));
        result.Set("key", doc2.RootElement.GetProperty("value"));

        Assert.That(result.GetInt("key"), Is.EqualTo(2));
    }

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
