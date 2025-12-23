using System.Net;
using System.Text;
using System.Text.Json;
using DevBase.Net;
using DevBase.Net.Configuration;
using DevBase.Net.Core;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests.Integration;

[TestFixture]
[Category("Integration")]
public class RequestIntegrationTest
{
    private MockHttpServer _server = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new MockHttpServer();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _server.ResetCounters();
    }

    #region Basic HTTP Methods

    [Test]
    public async Task Get_SimpleJson_ReturnsExpectedResponse()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/json")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.ContentType, Does.Contain("application/json"));
        
        var content = await response.GetStringAsync();
        Assert.That(content, Does.Contain("Hello, World!"));
        Assert.That(content, Does.Contain("success"));
    }

    [Test]
    public async Task Get_UsersList_ReturnsMultipleUsers()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        
        var usersArray = doc.RootElement.GetProperty("users");
        Assert.That(usersArray.GetArrayLength(), Is.EqualTo(3));
        Assert.That(doc.RootElement.GetProperty("total").GetInt32(), Is.EqualTo(3));
    }

    [Test]
    public async Task Get_SingleUser_ReturnsUserDetails()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users/1")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        
        Assert.That(doc.RootElement.GetProperty("id").GetInt32(), Is.EqualTo(1));
        Assert.That(doc.RootElement.GetProperty("name").GetString(), Is.EqualTo("Alice"));
        Assert.That(doc.RootElement.GetProperty("email").GetString(), Is.EqualTo("alice@example.com"));
    }

    [Test]
    public async Task Get_NonExistentUser_Returns404()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users/999")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        
        var content = await response.GetStringAsync();
        Assert.That(content, Does.Contain("User not found"));
    }

    [Test]
    public async Task Post_CreateUser_Returns201WithLocation()
    {
        // Arrange
        var userData = new { name = "NewUser", email = "newuser@example.com" };
        var request = new Request($"{_server.BaseUrl}/api/users")
            .AsPost()
            .WithJsonBody(JsonSerializer.Serialize(userData), Encoding.UTF8)
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Contains("Location"), Is.True);
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.That(doc.RootElement.GetProperty("created").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Put_UpdateUser_ReturnsUpdatedUser()
    {
        // Arrange
        var userData = new { name = "UpdatedAlice", email = "updated@example.com" };
        var request = new Request($"{_server.BaseUrl}/api/users/1")
            .AsPut()
            .WithJsonBody(JsonSerializer.Serialize(userData), Encoding.UTF8)
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.That(doc.RootElement.GetProperty("updated").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Delete_User_Returns204NoContent()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/users/1")
            .AsDelete()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Patch_PartialUpdate_ReturnsPatched()
    {
        // Arrange
        var patchData = new { name = "PatchedAlice" };
        var request = new Request($"{_server.BaseUrl}/api/users/1")
            .AsPatch()
            .WithJsonBody(JsonSerializer.Serialize(patchData), Encoding.UTF8)
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.That(doc.RootElement.GetProperty("patched").GetBoolean(), Is.True);
    }

    #endregion

    #region Headers

    [Test]
    public async Task Get_WithCustomHeaders_ServerReceivesHeaders()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/headers")
            .AsGet()
            .WithHeader("X-Custom-Header", "CustomValue")
            .WithHeader("X-Another-Header", "AnotherValue")
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        var headers = doc.RootElement.GetProperty("headers");
        
        Assert.That(headers.GetProperty("X-Custom-Header").GetString(), Is.EqualTo("CustomValue"));
        Assert.That(headers.GetProperty("X-Another-Header").GetString(), Is.EqualTo("AnotherValue"));
    }

    [Test]
    public async Task Get_WithUserAgent_ServerReceivesUserAgent()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/headers")
            .AsGet()
            .WithUserAgent("TestAgent/1.0")
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        var headers = doc.RootElement.GetProperty("headers");
        
        Assert.That(headers.GetProperty("User-Agent").GetString(), Is.EqualTo("TestAgent/1.0"));
    }

    [Test]
    public async Task Get_WithAcceptHeader_ServerReceivesAccept()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/headers")
            .AsGet()
            .WithAccept("application/json")
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        var headers = doc.RootElement.GetProperty("headers");
        
        Assert.That(headers.GetProperty("Accept").GetString(), Is.EqualTo("application/json"));
    }

    #endregion

    #region Query Parameters

    [Test]
    public async Task Get_WithQueryParameters_ServerReceivesParameters()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/query?name=test&page=1&limit=10")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        var query = doc.RootElement.GetProperty("query");
        
        Assert.That(query.GetProperty("name").GetString(), Is.EqualTo("test"));
        Assert.That(query.GetProperty("page").GetString(), Is.EqualTo("1"));
        Assert.That(query.GetProperty("limit").GetString(), Is.EqualTo("10"));
    }

    [Test]
    public async Task Get_WithSpecialCharactersInQuery_EncodesCorrectly()
    {
        // Arrange - use URL-encoded values directly
        var request = new Request($"{_server.BaseUrl}/api/query?search=hello%20world&special=test")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        var query = doc.RootElement.GetProperty("query");
        
        Assert.That(query.GetProperty("search").GetString(), Is.EqualTo("hello world"));
        Assert.That(query.GetProperty("special").GetString(), Is.EqualTo("test"));
    }

    #endregion

    #region Authentication

    [Test]
    public async Task Get_WithoutAuth_Returns401()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/auth")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Get_WithBearerToken_ReturnsAuthenticated()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/auth")
            .AsGet()
            .UseBearerAuthentication("my-secret-token")
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.That(doc.RootElement.GetProperty("authenticated").GetBoolean(), Is.True);
        Assert.That(doc.RootElement.GetProperty("authHeader").GetString(), Does.Contain("Bearer my-secret-token"));
    }

    [Test]
    public async Task Get_WithBasicAuth_ReturnsAuthenticated()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/auth")
            .AsGet()
            .UseBasicAuthentication("testuser", "testpass")
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        Assert.That(doc.RootElement.GetProperty("authenticated").GetBoolean(), Is.True);
        Assert.That(doc.RootElement.GetProperty("authHeader").GetString(), Does.StartWith("Basic"));
    }

    #endregion

    #region Error Handling

    [Test]
    public async Task Get_ServerError500_ReturnsErrorResponse()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/error/500")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        
        var content = await response.GetStringAsync();
        Assert.That(content, Does.Contain("Internal Server Error"));
    }

    [Test]
    public async Task Get_NotFoundEndpoint_Returns404()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/nonexistent")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    #endregion

    #region Content Types

    [Test]
    public async Task Get_PlainText_ReturnsTextContent()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/text")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.ContentType, Does.Contain("text/plain"));
        
        var content = await response.GetStringAsync();
        Assert.That(content, Is.EqualTo("This is plain text response"));
    }

    [Test]
    public async Task Get_XmlContent_ReturnsXml()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/xml")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.ContentType, Does.Contain("application/xml"));
        
        var content = await response.GetStringAsync();
        Assert.That(content, Does.Contain("<message>Hello XML</message>"));
    }

    #endregion

    #region Large Responses

    [Test]
    public async Task Get_LargeResponse_HandlesCorrectly()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/large")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        
        Assert.That(doc.RootElement.GetProperty("count").GetInt32(), Is.EqualTo(1000));
        var items = doc.RootElement.GetProperty("items");
        Assert.That(items.GetArrayLength(), Is.EqualTo(1000));
    }

    #endregion

    #region Nested JSON

    [Test]
    public async Task Get_NestedJson_ParsesCorrectly()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/nested")
            .AsGet()
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        
        var value = doc.RootElement
            .GetProperty("level1")
            .GetProperty("level2")
            .GetProperty("level3")
            .GetProperty("value")
            .GetInt32();
        
        Assert.That(value, Is.EqualTo(42));
    }

    #endregion

    #region Form Data

    [Test]
    public async Task Post_FormData_ServerReceivesFormData()
    {
        // Arrange - Use raw body with form encoding
        var formData = "field1=value1&field2=value2";
        var request = new Request($"{_server.BaseUrl}/api/form")
            .AsPost()
            .WithRawBody(formData, System.Text.Encoding.UTF8)
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        using var doc = JsonDocument.Parse(content);
        
        // Verify body was received
        Assert.That(doc.RootElement.GetProperty("body").GetString(), 
            Does.Contain("field1=value1"));
    }

    #endregion

    #region Echo

    [Test]
    public async Task Post_Echo_ReturnsSameContent()
    {
        // Arrange
        var originalContent = "This is the original content to echo back";
        var request = new Request($"{_server.BaseUrl}/api/echo")
            .AsPost()
            .WithRawBody(originalContent, Encoding.UTF8)
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.GetStringAsync();
        Assert.That(content, Is.EqualTo(originalContent));
    }

    #endregion

    #region Request Recording

    [Test]
    public async Task MultipleRequests_AllRecorded()
    {
        // Arrange & Act
        await new Request($"{_server.BaseUrl}/api/json").AsGet().Build().SendAsync();
        await new Request($"{_server.BaseUrl}/api/users").AsGet().Build().SendAsync();
        await new Request($"{_server.BaseUrl}/api/users/1").AsGet().Build().SendAsync();

        // Assert
        Assert.That(_server.RequestCount, Is.EqualTo(3));
        Assert.That(_server.RecordedRequests, Has.Count.EqualTo(3));
        Assert.That(_server.RecordedRequests[0].Path, Is.EqualTo("/api/json"));
        Assert.That(_server.RecordedRequests[1].Path, Is.EqualTo("/api/users"));
        Assert.That(_server.RecordedRequests[2].Path, Is.EqualTo("/api/users/1"));
    }

    #endregion

    #region Timeout

    [Test]
    public async Task Get_WithTimeout_CompletesBeforeTimeout()
    {
        // Arrange
        var request = new Request($"{_server.BaseUrl}/api/delay/100")
            .AsGet()
            .WithTimeout(TimeSpan.FromSeconds(5))
            .Build();

        // Act
        var response = await request.SendAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Cancellation

    [Test]
    public async Task Get_WithCancellation_ThrowsOnCancel()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var request = new Request($"{_server.BaseUrl}/api/delay/500")
            .AsGet()
            .Build();

        // Act
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Assert
        Assert.ThrowsAsync<TaskCanceledException>(async () => 
            await request.SendAsync(cts.Token));
    }

    #endregion
}
