using System.Net;
using System.Text;
using System.Text.Json;
using DevBase.Net;
using DevBase.Net.Core;
using DevBase.Net.Parsing;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests.Integration;

[TestFixture]
[Category("Integration")]
public class JsonPathParserIntegrationTest
{
    private MockHttpServer _server = null!;
    private JsonPathParser _parser = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new MockHttpServer();
        _parser = new JsonPathParser();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Dispose();
    }

    #region Basic JSON Path Parsing

    [Test]
    public async Task Parse_SimpleProperty_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var message = _parser.Parse<string>(bytes, "$.message");

        // Assert
        Assert.That(message, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public async Task Parse_BooleanProperty_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var success = _parser.Parse<bool>(bytes, "$.success");

        // Assert
        Assert.That(success, Is.True);
    }

    [Test]
    public async Task Parse_ArrayElement_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var firstUser = _parser.Parse<JsonElement>(bytes, "$.users[0]");

        // Assert
        Assert.That(firstUser.GetProperty("id").GetInt32(), Is.EqualTo(1));
        Assert.That(firstUser.GetProperty("name").GetString(), Is.EqualTo("Alice"));
    }

    [Test]
    public async Task Parse_NestedProperty_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/nested")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var value = _parser.Parse<int>(bytes, "$.level1.level2.level3.value");

        // Assert
        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public async Task Parse_DeepNestedArray_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/nested")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var firstItem = _parser.Parse<string>(bytes, "$.level1.level2.level3.data[0]");

        // Assert
        Assert.That(firstItem, Is.EqualTo("a"));
    }

    #endregion

    #region Parse List

    [Test]
    public async Task ParseList_AllUsers_ReturnsAllItems()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var users = _parser.ParseList<JsonElement>(bytes, "$.users[*]");

        // Assert
        Assert.That(users, Has.Count.EqualTo(3));
        Assert.That(users[0].GetProperty("name").GetString(), Is.EqualTo("Alice"));
        Assert.That(users[1].GetProperty("name").GetString(), Is.EqualTo("Bob"));
        Assert.That(users[2].GetProperty("name").GetString(), Is.EqualTo("Charlie"));
    }

    [Test]
    public async Task ParseList_UserNames_ReturnsAllNames()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        
        // Get all user objects first, then extract names
        var users = _parser.ParseList<JsonElement>(bytes, "$.users[*]");
        var names = users.Select(u => u.GetProperty("name").GetString()).ToList();

        // Assert
        Assert.That(names, Contains.Item("Alice"));
        Assert.That(names, Contains.Item("Bob"));
        Assert.That(names, Contains.Item("Charlie"));
    }

    #endregion

    #region Large Response Parsing

    [Test]
    public async Task Parse_LargeResponse_TotalCount_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/large")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var count = _parser.Parse<int>(bytes, "$.count");

        // Assert
        Assert.That(count, Is.EqualTo(1000));
    }

    [Test]
    public async Task Parse_LargeResponse_FirstItem_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/large")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var firstItem = _parser.Parse<JsonElement>(bytes, "$.items[0]");

        // Assert
        Assert.That(firstItem.GetProperty("id").GetInt32(), Is.EqualTo(1));
        Assert.That(firstItem.GetProperty("name").GetString(), Is.EqualTo("Item 1"));
    }

    [Test]
    public async Task Parse_LargeResponse_LastItem_ReturnsValue()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/large")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var lastItem = _parser.Parse<JsonElement>(bytes, "$.items[999]");

        // Assert
        Assert.That(lastItem.GetProperty("id").GetInt32(), Is.EqualTo(1000));
        Assert.That(lastItem.GetProperty("name").GetString(), Is.EqualTo("Item 1000"));
    }

    #endregion

    #region Error Cases

    [Test]
    public async Task Parse_NonExistentPath_ReturnsDefault()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var result = _parser.Parse<string>(bytes, "$.nonexistent");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Parse_InvalidArrayIndex_ReturnsDefault()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        var result = _parser.Parse<JsonElement>(bytes, "$.users[999]");

        // Assert
        Assert.That(result.ValueKind, Is.EqualTo(JsonValueKind.Undefined));
    }

    #endregion

    #region Complex Scenarios

    [Test]
    public async Task Parse_UserById_ThenParseDetails()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users/1")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();
        var bytes = await response.GetBytesAsync();
        
        var id = _parser.Parse<int>(bytes, "$.id");
        var name = _parser.Parse<string>(bytes, "$.name");
        var email = _parser.Parse<string>(bytes, "$.email");
        var active = _parser.Parse<bool>(bytes, "$.active");

        // Assert
        Assert.That(id, Is.EqualTo(1));
        Assert.That(name, Is.EqualTo("Alice"));
        Assert.That(email, Is.EqualTo("alice@example.com"));
        Assert.That(active, Is.True);
    }

    #endregion
}
