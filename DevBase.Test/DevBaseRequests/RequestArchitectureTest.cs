using System.Text;
using DevBase.IO;
using DevBase.Net.Core;
using DevBase.Net.Data.Body;
using DevBase.Net.Objects;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

/// <summary>
/// Tests for the refactored Request/Response architecture including:
/// - BaseRequest and BaseResponse inheritance
/// - RequestContent partial class methods
/// - MultipartFormBuilder functionality
/// </summary>
[TestFixture]
public class RequestArchitectureTest
{
    #region BaseRequest Inheritance Tests
    
    [Test]
    public void Request_InheritsFromBaseRequest()
    {
        var request = new Request("https://example.com");
        Assert.That(request, Is.InstanceOf<BaseRequest>());
    }
    
    [Test]
    public void Request_HasDefaultMethod_Get()
    {
        var request = new Request("https://example.com");
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
    }
    
    [Test]
    public void Request_HasDefaultTimeout_30Seconds()
    {
        var request = new Request("https://example.com");
        Assert.That(request.Timeout, Is.EqualTo(TimeSpan.FromSeconds(30)));
    }
    
    [Test]
    public void Request_HasDefaultRetryPolicy_None()
    {
        var request = new Request("https://example.com");
        Assert.That(request.RetryPolicy.MaxRetries, Is.EqualTo(0));
    }
    
    [Test]
    public void Request_ImplementsIDisposable()
    {
        var request = new Request("https://example.com");
        Assert.That(request, Is.InstanceOf<IDisposable>());
        Assert.DoesNotThrow(() => request.Dispose());
    }
    
    [Test]
    public void Request_ImplementsIAsyncDisposable()
    {
        var request = new Request("https://example.com");
        Assert.That(request, Is.InstanceOf<IAsyncDisposable>());
        Assert.DoesNotThrow(() => request.DisposeAsync());
    }
    
    [Test]
    public void Request_Uri_ReturnsConfiguredUrl()
    {
        var request = new Request("https://example.com/api/test");
        Assert.That(request.Uri.ToString(), Does.Contain("example.com"));
    }
    
    [Test]
    public void Request_CanBeCreatedWithHttpMethod()
    {
        var request = new Request("https://example.com", HttpMethod.Post);
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
    }
    
    #endregion
    
    #region RequestContent Tests
    
    [Test]
    public void WithTextContent_SetsBodyAndContentType()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .WithTextContent("Hello World");
        
        request.Build();
        
        Assert.That(request.HasContent(), Is.True);
        Assert.That(request.GetContentType(), Does.Contain("text/plain"));
    }
    
    [Test]
    public void WithXmlContent_SetsBodyAndContentType()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .WithXmlContent("<root><item>test</item></root>");
        
        request.Build();
        
        Assert.That(request.HasContent(), Is.True);
        Assert.That(request.GetContentType(), Does.Contain("application/xml"));
    }
    
    [Test]
    public void WithHtmlContent_SetsBodyAndContentType()
    {
        var request = new Request("https://example.com")
            .AsPost()
            .WithHtmlContent("<html><body>Test</body></html>");
        
        request.Build();
        
        Assert.That(request.HasContent(), Is.True);
        Assert.That(request.GetContentType(), Does.Contain("text/html"));
    }
    
    [Test]
    public void WithBinaryContent_SetsBody()
    {
        byte[] data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var request = new Request("https://example.com")
            .AsPost()
            .WithBufferBody(data);
        
        request.Build();
        
        Assert.That(request.HasContent(), Is.True);
        Assert.That(request.GetContentLength(), Is.EqualTo(4));
    }
    
    [Test]
    public void WithBinaryContent_WithContentType_SetsContentType()
    {
        byte[] data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var request = new Request("https://example.com")
            .AsPost()
            .WithBufferBody(data)
            .WithHeader("Content-Type", "application/octet-stream");
        
        request.Build();
        
        Assert.That(request.GetContentType(), Is.EqualTo("application/octet-stream"));
    }
    
    [Test]
    public void HasContent_ReturnsFalse_WhenNoBodySet()
    {
        var request = new Request("https://example.com");
        Assert.That(request.HasContent(), Is.False);
    }
    
    [Test]
    public void GetContentLength_ReturnsZero_WhenNoBodySet()
    {
        var request = new Request("https://example.com");
        Assert.That(request.GetContentLength(), Is.EqualTo(0));
    }
    
    [Test]
    public void WithFileContent_FromBytes_CreatesMultipartBody()
    {
        byte[] fileData = Encoding.UTF8.GetBytes("test file content");
        var fileObject = AFileObject.FromBuffer(fileData, "test.txt");
        
        var request = new Request("https://example.com/upload")
            .AsPost()
            .WithFileContent(fileObject, "document");
        
        request.Build();
        
        Assert.That(request.HasContent(), Is.True);
        string body = Encoding.UTF8.GetString(request.Body.ToArray());
        Assert.That(body, Does.Contain("Content-Disposition: form-data"));
        Assert.That(body, Does.Contain("name=\"document\""));
    }
    
    #endregion
    
    #region MultipartFormBuilder Tests
    
    [Test]
    public void MultipartFormBuilder_AddField_AddsTextEntry()
    {
        var builder = new MultipartFormBuilder();
        builder.AddField("username", "john_doe");
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"username\""));
        Assert.That(body, Does.Contain("john_doe"));
    }
    
    [Test]
    public void MultipartFormBuilder_AddFields_AddsMultipleEntries()
    {
        var builder = new MultipartFormBuilder();
        builder.AddFields(
            ("field1", "value1"),
            ("field2", "value2"),
            ("field3", "value3")
        );
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"field1\""));
        Assert.That(body, Does.Contain("name=\"field2\""));
        Assert.That(body, Does.Contain("name=\"field3\""));
    }
    
    [Test]
    public void MultipartFormBuilder_AddFile_WithBytes_AddsFileEntry()
    {
        byte[] fileData = Encoding.UTF8.GetBytes("file content");
        var builder = new MultipartFormBuilder();
        builder.AddFile("document", fileData, "test.txt");
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"document\""));
        Assert.That(body, Does.Contain("filename=\"test.txt\""));
        Assert.That(body, Does.Contain("file content"));
    }
    
    [Test]
    public void MultipartFormBuilder_AddFile_WithAFileObject_AddsEntry()
    {
        byte[] fileData = Encoding.UTF8.GetBytes("test data");
        var fileObject = AFileObject.FromBuffer(fileData, "data.json");
        
        var builder = new MultipartFormBuilder();
        builder.AddFile("jsonFile", fileObject);
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"jsonFile\""));
        Assert.That(body, Does.Contain("test data"));
    }
    
    [Test]
    public void MultipartFormBuilder_BoundaryString_IsNotEmpty()
    {
        var builder = new MultipartFormBuilder();
        Assert.That(builder.BoundaryString, Is.Not.Empty);
    }
    
    [Test]
    public void MultipartFormBuilder_Count_ReflectsEntries()
    {
        var builder = new MultipartFormBuilder();
        builder.AddField("field1", "value1");
        builder.AddField("field2", "value2");
        
        Assert.That(builder.Count, Is.EqualTo(2));
    }
    
    [Test]
    public void MultipartFormBuilder_RemoveField_RemovesEntry()
    {
        var builder = new MultipartFormBuilder();
        builder.AddField("keep", "value1");
        builder.AddField("remove", "value2");
        builder.RemoveField("remove");
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
        Assert.That(body, Does.Contain("keep"));
        Assert.That(body, Does.Not.Contain("name=\"remove\""));
    }
    
    [Test]
    public void MultipartFormBuilder_AddBinaryData_AddsBinaryEntry()
    {
        byte[] data = new byte[] { 0x00, 0x01, 0x02, 0xFF };
        var builder = new MultipartFormBuilder();
        builder.AddBinaryData("binary", data);
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        byte[] body = formBuilder.Buffer.ToArray();
        Assert.That(ContainsSequence(body, data), Is.True);
    }
    
    [Test]
    public void MultipartFormBuilder_FromFile_CreatesBuilderWithFile()
    {
        // Create temp file for testing
        string tempPath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempPath, "test content");
            
            var builder = MultipartFormBuilder.FromFile("upload", tempPath);
            var formBuilder = builder.Build();
            formBuilder.Build();
            
            string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
            Assert.That(body, Does.Contain("name=\"upload\""));
            Assert.That(body, Does.Contain("test content"));
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
    
    [Test]
    public void MultipartFormBuilder_FromFields_CreatesBuilderWithFields()
    {
        var builder = MultipartFormBuilder.FromFields(
            ("name", "John"),
            ("email", "john@example.com")
        );
        
        var formBuilder = builder.Build();
        formBuilder.Build();
        
        string body = Encoding.UTF8.GetString(formBuilder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"name\""));
        Assert.That(body, Does.Contain("John"));
        Assert.That(body, Does.Contain("name=\"email\""));
        Assert.That(body, Does.Contain("john@example.com"));
    }
    
    #endregion
    
    #region Request WithMultipartForm Tests
    
    [Test]
    public void Request_WithMultipartForm_BuildsCorrectBody()
    {
        var request = new Request("https://example.com/upload")
            .AsPost()
            .WithMultipartForm(form =>
            {
                form.AddField("username", "testuser");
                form.AddFile("avatar", Encoding.UTF8.GetBytes("fake image"), "avatar.png");
            });
        
        request.Build();
        
        Assert.That(request.HasContent(), Is.True);
        string body = Encoding.UTF8.GetString(request.Body.ToArray());
        Assert.That(body, Does.Contain("name=\"username\""));
        Assert.That(body, Does.Contain("testuser"));
        Assert.That(body, Does.Contain("name=\"avatar\""));
        Assert.That(body, Does.Contain("avatar.png"));
    }
    
    [Test]
    public void Request_WithSingleFileUpload_CreatesMultipartRequest()
    {
        string tempPath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempPath, "document content");
            
            var request = new Request("https://example.com/upload")
                .AsPost()
                .WithSingleFileUpload("document", tempPath);
            
            request.Build();
            
            Assert.That(request.HasContent(), Is.True);
            string body = Encoding.UTF8.GetString(request.Body.ToArray());
            Assert.That(body, Does.Contain("name=\"document\""));
            Assert.That(body, Does.Contain("document content"));
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
    
    [Test]
    public void Request_WithSingleFileUpload_WithAdditionalFields_IncludesAll()
    {
        string tempPath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempPath, "file data");
            
            var request = new Request("https://example.com/upload")
                .AsPost()
                .WithSingleFileUpload("file", tempPath, 
                    ("description", "My file"),
                    ("category", "documents"));
            
            request.Build();
            
            string body = Encoding.UTF8.GetString(request.Body.ToArray());
            Assert.That(body, Does.Contain("name=\"file\""));
            Assert.That(body, Does.Contain("name=\"description\""));
            Assert.That(body, Does.Contain("My file"));
            Assert.That(body, Does.Contain("name=\"category\""));
            Assert.That(body, Does.Contain("documents"));
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
    
    #endregion
    
    #region Helper Methods
    
    private static bool ContainsSequence(byte[] source, byte[] pattern)
    {
        for (int i = 0; i <= source.Length - pattern.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (source[i + j] != pattern[j])
                {
                    found = false;
                    break;
                }
            }
            if (found) return true;
        }
        return false;
    }
    
    #endregion
}
