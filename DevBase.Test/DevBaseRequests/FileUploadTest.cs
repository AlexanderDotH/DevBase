using System.Text;
using DevBase.IO;
using DevBase.Net.Core;
using DevBase.Net.Data.Body;
using DevBase.Net.Objects;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class FileUploadTest
{
    #region Builder Construction

    [Test]
    public void Constructor_GeneratesUniqueBoundary()
    {
        var builder1 = new RequestKeyValueListBodyBuilder();
        var builder2 = new RequestKeyValueListBodyBuilder();

        Assert.That(builder1.BoundaryString, Is.Not.Empty);
        Assert.That(builder2.BoundaryString, Is.Not.Empty);
        Assert.That(builder1.BoundaryString, Is.Not.EqualTo(builder2.BoundaryString));
    }

    [Test]
    public void BoundaryString_ContainsExpectedFormat()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        string boundary = builder.BoundaryString;

        Assert.That(boundary, Does.StartWith("--------------------------"));
        Assert.That(boundary.Length, Is.GreaterThan(26));
    }

    [Test]
    public void Bounds_Separator_Tail_AreInitialized()
    {
        var builder = new RequestKeyValueListBodyBuilder();

        Assert.That(builder.Bounds.IsEmpty, Is.False);
        Assert.That(builder.Separator.IsEmpty, Is.False);
        Assert.That(builder.Tail.IsEmpty, Is.False);
    }

    #endregion

    #region Adding Files

    [Test]
    public void AddFile_WithFieldNameAndBytes_AddsEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        byte[] fileData = Encoding.UTF8.GetBytes("test file content");

        builder.AddFile("myFile", fileData);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"myFile\""));
        Assert.That(body, Does.Contain("test file content"));
    }

    [Test]
    public void AddFile_WithMimeFileObject_AddsEntryWithMimeType()
    {
        byte[] fileData = Encoding.UTF8.GetBytes("{\"key\": \"value\"}");
        var fileObject = AFileObject.FromBuffer(fileData, "data.json");
        var mimeFile = MimeFileObject.FromAFileObject(fileObject);
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddFile("jsonFile", mimeFile);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"jsonFile\""));
        Assert.That(body, Does.Contain("filename=\"data.json\""));
        Assert.That(body, Does.Contain("Content-Type: application/json"));
    }

    [Test]
    public void AddFile_WithoutFieldName_UsesFilenameAsFieldName()
    {
        byte[] fileData = Encoding.UTF8.GetBytes("image data");
        var fileObject = AFileObject.FromBuffer(fileData, "photo.png");
        var mimeFile = MimeFileObject.FromAFileObject(fileObject);
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddFile(mimeFile);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"photo.png\""));
    }

    #endregion

    #region Adding Text Fields

    [Test]
    public void AddText_AddsTextEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddText("username", "john_doe");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"username\""));
        Assert.That(body, Does.Contain("john_doe"));
    }

    [Test]
    public void AddText_MultipleFields_AddsAllEntries()
    {
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddText("field1", "value1");
        builder.AddText("field2", "value2");
        builder.AddText("field3", "value3");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"field1\""));
        Assert.That(body, Does.Contain("value1"));
        Assert.That(body, Does.Contain("name=\"field2\""));
        Assert.That(body, Does.Contain("value2"));
        Assert.That(body, Does.Contain("name=\"field3\""));
        Assert.That(body, Does.Contain("value3"));
    }

    #endregion

    #region Mixed Content

    [Test]
    public void Build_MixedFileAndText_ContainsBothTypes()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        byte[] fileData = Encoding.UTF8.GetBytes("file content here");

        builder.AddText("description", "My uploaded file");
        builder.AddFile("document", fileData);
        builder.AddText("category", "documents");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        
        Assert.That(body, Does.Contain("name=\"description\""));
        Assert.That(body, Does.Contain("My uploaded file"));
        Assert.That(body, Does.Contain("name=\"document\""));
        Assert.That(body, Does.Contain("file content here"));
        Assert.That(body, Does.Contain("name=\"category\""));
        Assert.That(body, Does.Contain("documents"));
    }

    #endregion

    #region RFC 2046 Multipart Format Compliance

    [Test]
    public void Build_ContainsContentDispositionHeaders()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("field", "value");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("Content-Disposition: form-data;"));
    }

    [Test]
    public void Build_ContainsBoundaryMarkers()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("field", "value");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        string boundary = builder.BoundaryString;

        // Body contains the boundary string and ends with closing boundary
        Assert.That(body, Does.Contain(boundary));
        Assert.That(body.TrimEnd(), Does.EndWith("--")); // Multipart ends with --
    }

    [Test]
    public void Build_FileEntry_ContainsFilenameAndContentType()
    {
        byte[] fileData = Encoding.UTF8.GetBytes("test");
        var fileObject = AFileObject.FromBuffer(fileData, "test.txt");
        var mimeFile = MimeFileObject.FromAFileObject(fileObject);
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddFile("upload", mimeFile);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("filename=\"test.txt\""));
        Assert.That(body, Does.Contain("Content-Type:"));
    }

    [Test]
    public void Build_ClosingBoundary_EndsWithDoubleDash()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("field", "value");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        
        // RFC 2046: Closing boundary ends with "--" suffix
        Assert.That(body.TrimEnd(), Does.EndWith("--"));
        // Verify body contains multipart structure
        Assert.That(body, Does.Contain("Content-Disposition: form-data"));
    }

    #endregion

    #region Request Integration

    [Test]
    public void Request_WithForm_HasCorrectBodyStructure()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("test", "value");
        
        var request = new Request("https://example.com/upload")
            .AsPost()
            .WithForm(builder);
        
        request.Build();

        // Verify the body contains proper multipart structure
        string body = Encoding.UTF8.GetString(request.Body.ToArray());
        Assert.That(body, Does.Contain("Content-Disposition: form-data"));
        Assert.That(body, Does.Contain(builder.BoundaryString));
        Assert.That(body.TrimEnd(), Does.EndWith("--"));
    }

    [Test]
    public void Request_WithForm_BoundaryStringAccessible()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("test", "value");

        // BoundaryString can be used to manually construct Content-Type header
        string contentType = $"multipart/form-data; boundary={builder.BoundaryString}";
        
        Assert.That(builder.BoundaryString, Is.Not.Empty);
        Assert.That(contentType, Does.Contain(builder.BoundaryString));
        Assert.That(contentType, Does.StartWith("multipart/form-data"));
    }

    [Test]
    public void Request_WithForm_BodyContainsFormData()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("username", "testuser");
        builder.AddFile("avatar", Encoding.UTF8.GetBytes("fake image data"));

        var request = new Request("https://example.com/upload")
            .AsPost()
            .WithForm(builder);

        Assert.That(request.Body.IsEmpty, Is.False);
        string body = Encoding.UTF8.GetString(request.Body.ToArray());
        Assert.That(body, Does.Contain("username"));
        Assert.That(body, Does.Contain("testuser"));
        Assert.That(body, Does.Contain("avatar"));
    }

    [Test]
    public void Request_WithCustomContentType_DoesNotOverwrite()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("test", "value");

        var request = new Request("https://example.com/upload")
            .AsPost()
            .WithHeader("Content-Type", "custom/type")
            .WithForm(builder);
        
        request.Build();
        var httpMessage = request.ToHttpRequestMessage();

        // Custom Content-Type is in request headers, not message headers
        // The Content-Type should not be overwritten by multipart detection
        Assert.That(httpMessage.Content!.Headers.ContentType, Is.Null);
    }

    #endregion

    #region Entry Management

    [Test]
    public void RemoveEntryAt_RemovesEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("field1", "value1");
        builder.AddText("field2", "value2");
        builder.RemoveEntryAt(0);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Not.Contain("field1"));
        Assert.That(body, Does.Contain("field2"));
    }

    [Test]
    public void Remove_ByFieldName_RemovesEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("keep", "value1");
        builder.AddText("remove", "value2");
        builder.Remove("remove");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("keep"));
        Assert.That(body, Does.Not.Contain("remove"));
    }

    [Test]
    public void Indexer_SetNull_RemovesEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddText("field", "value");
        builder["field"] = null;
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Not.Contain("name=\"field\""));
    }

    [Test]
    public void Indexer_SetString_AddsTextEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder["newField"] = "newValue";
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"newField\""));
        Assert.That(body, Does.Contain("newValue"));
    }

    [Test]
    public void Indexer_SetBytes_AddsFileEntry()
    {
        var builder = new RequestKeyValueListBodyBuilder();
        builder["fileField"] = Encoding.UTF8.GetBytes("file content");
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("name=\"fileField\""));
        Assert.That(body, Does.Contain("file content"));
    }

    #endregion

    #region Binary File Handling

    [Test]
    public void AddFile_BinaryData_PreservesBytes()
    {
        byte[] binaryData = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0xFE, 0xFD };
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddFile("binary", binaryData);
        builder.Build();

        byte[] body = builder.Buffer.ToArray();
        // Verify binary data is contained in the body
        bool containsData = ContainsSequence(body, binaryData);
        Assert.That(containsData, Is.True);
    }
    
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

    [Test]
    public void Build_LargeFile_HandlesCorrectly()
    {
        byte[] largeData = new byte[1024 * 1024]; // 1 MB
        new Random(42).NextBytes(largeData);

        var builder = new RequestKeyValueListBodyBuilder();
        builder.AddFile("largefile", largeData);
        builder.Build();

        Assert.That(builder.Buffer.Length, Is.GreaterThan(largeData.Length));
    }

    #endregion

    #region MIME Type Detection

    [Test]
    public void AddFile_JsonExtension_DetectsJsonMimeType()
    {
        byte[] data = Encoding.UTF8.GetBytes("{}");
        var fileObject = AFileObject.FromBuffer(data, "config.json");
        var mimeFile = MimeFileObject.FromAFileObject(fileObject);
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddFile("config", mimeFile);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("Content-Type: application/json"));
    }

    [Test]
    public void AddFile_PngExtension_DetectsImageMimeType()
    {
        byte[] data = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG magic bytes
        var fileObject = AFileObject.FromBuffer(data, "image.png");
        var mimeFile = MimeFileObject.FromAFileObject(fileObject);
        var builder = new RequestKeyValueListBodyBuilder();

        builder.AddFile("image", mimeFile);
        builder.Build();

        string body = Encoding.UTF8.GetString(builder.Buffer.ToArray());
        Assert.That(body, Does.Contain("Content-Type: image/png"));
    }

    #endregion
}
