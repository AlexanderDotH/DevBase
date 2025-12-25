using System.Text;
using System.Text.Json;
using DevBase.IO;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Body.Mime;
using DevBase.Net.Objects;

namespace DevBase.Net.Core;

/// <summary>
/// Partial class for Request handling content and file operations.
/// Provides methods for setting various types of request content including files, streams, and raw data.
/// </summary>
public partial class Request
{
    #region File Content from AFile/AFileObject
    
    /// <summary>
    /// Sets the request body from an AFileObject with automatic MIME type detection.
    /// </summary>
    /// <param name="fileObject">The file object containing file data and metadata.</param>
    /// <param name="fieldName">Optional field name for multipart forms. Defaults to filename.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithFileContent(AFileObject fileObject, string? fieldName = null)
    {
        ArgumentNullException.ThrowIfNull(fileObject);
        
        MimeFileObject mimeFile = MimeFileObject.FromAFileObject(fileObject);
        return this.WithFileContent(mimeFile, fieldName);
    }
    
    /// <summary>
    /// Sets the request body from a MimeFileObject.
    /// </summary>
    /// <param name="mimeFile">The MIME file object containing file data and type information.</param>
    /// <param name="fieldName">Optional field name for multipart forms. Defaults to filename.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithFileContent(MimeFileObject mimeFile, string? fieldName = null)
    {
        ArgumentNullException.ThrowIfNull(mimeFile);
        
        RequestKeyValueListBodyBuilder formBuilder = new RequestKeyValueListBodyBuilder();
        formBuilder.AddFile(fieldName ?? mimeFile.FileInfo?.Name ?? "file", mimeFile);
        return this.WithForm(formBuilder);
    }
    
    /// <summary>
    /// Sets the request body from a FileInfo with automatic MIME type detection.
    /// </summary>
    /// <param name="fileInfo">The file information.</param>
    /// <param name="fieldName">Optional field name for multipart forms. Defaults to filename.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithFileContent(FileInfo fileInfo, string? fieldName = null)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        
        if (!fileInfo.Exists)
            throw new FileNotFoundException("File not found", fileInfo.FullName);
        
        Memory<byte> buffer = AFile.ReadFile(fileInfo);
        AFileObject fileObject = AFileObject.FromBuffer(buffer.ToArray(), fileInfo.Name);
        return this.WithFileContent(fileObject, fieldName);
    }
    
    /// <summary>
    /// Sets the request body from a file path with automatic MIME type detection.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="fieldName">Optional field name for multipart forms. Defaults to filename.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithFileContent(string filePath, string? fieldName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        return this.WithFileContent(new FileInfo(filePath), fieldName);
    }
    
    #endregion
    
    #region Multiple Files
    
    /// <summary>
    /// Sets the request body from multiple files.
    /// </summary>
    /// <param name="files">Array of tuples containing field name and file object pairs.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithMultipleFiles(params (string fieldName, AFileObject file)[] files)
    {
        ArgumentNullException.ThrowIfNull(files);
        
        RequestKeyValueListBodyBuilder formBuilder = new RequestKeyValueListBodyBuilder();
        foreach ((string fieldName, AFileObject file) in files)
        {
            MimeFileObject mimeFile = MimeFileObject.FromAFileObject(file);
            formBuilder.AddFile(fieldName, mimeFile);
        }
        return this.WithForm(formBuilder);
    }
    
    /// <summary>
    /// Sets the request body from multiple files with FileInfo.
    /// </summary>
    /// <param name="files">Array of tuples containing field name and FileInfo pairs.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithMultipleFiles(params (string fieldName, FileInfo file)[] files)
    {
        ArgumentNullException.ThrowIfNull(files);
        
        RequestKeyValueListBodyBuilder formBuilder = new RequestKeyValueListBodyBuilder();
        foreach ((string fieldName, FileInfo fileInfo) in files)
        {
            if (!fileInfo.Exists)
                throw new FileNotFoundException("File not found", fileInfo.FullName);
            
            Memory<byte> buffer = AFile.ReadFile(fileInfo);
            AFileObject fileObject = AFileObject.FromBuffer(buffer.ToArray(), fileInfo.Name);
            MimeFileObject mimeFile = MimeFileObject.FromAFileObject(fileObject);
            formBuilder.AddFile(fieldName, mimeFile);
        }
        return this.WithForm(formBuilder);
    }
    
    /// <summary>
    /// Sets the request body from multiple file paths.
    /// </summary>
    /// <param name="files">Array of tuples containing field name and file path pairs.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithMultipleFiles(params (string fieldName, string filePath)[] files)
    {
        ArgumentNullException.ThrowIfNull(files);
        
        var fileInfos = files.Select(f => (f.fieldName, new FileInfo(f.filePath))).ToArray();
        return this.WithMultipleFiles(fileInfos);
    }
    
    #endregion
    
    #region Stream Content
    
    /// <summary>
    /// Sets the request body from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the content.</param>
    /// <param name="contentType">Optional content type. Defaults to application/octet-stream.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithStreamContent(Stream stream, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        byte[] buffer = ms.ToArray();
        
        this.WithBufferBody(buffer);
        
        if (!string.IsNullOrEmpty(contentType))
            this.WithHeader("Content-Type", contentType);
        
        return this;
    }
    
    /// <summary>
    /// Sets the request body from a stream asynchronously.
    /// </summary>
    /// <param name="stream">The stream containing the content.</param>
    /// <param name="contentType">Optional content type. Defaults to application/octet-stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The request instance for method chaining.</returns>
    public async Task<Request> WithStreamContentAsync(Stream stream, string? contentType = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        
        using MemoryStream ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        byte[] buffer = ms.ToArray();
        
        this.WithBufferBody(buffer);
        
        if (!string.IsNullOrEmpty(contentType))
            this.WithHeader("Content-Type", contentType);
        
        return this;
    }
    
    #endregion
    
    #region Raw Binary Content
    
    /// <summary>
    /// Sets the request body from a Memory buffer.
    /// </summary>
    /// <param name="buffer">The memory buffer containing the content.</param>
    /// <param name="contentType">Optional content type.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithBinaryContent(Memory<byte> buffer, string? contentType = null)
    {
        this.WithBufferBody(buffer);
        
        if (!string.IsNullOrEmpty(contentType))
            this.WithHeader("Content-Type", contentType);
        
        return this;
    }
    
    /// <summary>
    /// Sets the request body from a ReadOnlyMemory buffer.
    /// </summary>
    /// <param name="buffer">The read-only memory buffer containing the content.</param>
    /// <param name="contentType">Optional content type.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithBinaryContent(ReadOnlyMemory<byte> buffer, string? contentType = null)
    {
        this.WithBufferBody(buffer.ToArray());
        
        if (!string.IsNullOrEmpty(contentType))
            this.WithHeader("Content-Type", contentType);
        
        return this;
    }
    
    /// <summary>
    /// Sets the request body from a Span buffer.
    /// </summary>
    /// <param name="buffer">The span containing the content.</param>
    /// <param name="contentType">Optional content type.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithBinaryContent(ReadOnlySpan<byte> buffer, string? contentType = null)
    {
        this.WithBufferBody(buffer.ToArray());
        
        if (!string.IsNullOrEmpty(contentType))
            this.WithHeader("Content-Type", contentType);
        
        return this;
    }
    
    #endregion
    
    #region Text Content
    
    /// <summary>
    /// Sets the request body as plain text.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <param name="encoding">Optional encoding. Defaults to UTF-8.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithTextContent(string text, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        encoding ??= Encoding.UTF8;
        
        this.WithRawBody(text, encoding);
        this.WithHeader("Content-Type", $"text/plain; charset={encoding.WebName}");
        
        return this;
    }
    
    /// <summary>
    /// Sets the request body as XML content.
    /// </summary>
    /// <param name="xml">The XML content.</param>
    /// <param name="encoding">Optional encoding. Defaults to UTF-8.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithXmlContent(string xml, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(xml);
        encoding ??= Encoding.UTF8;
        
        this.WithRawBody(xml, encoding);
        this.WithHeader("Content-Type", $"application/xml; charset={encoding.WebName}");
        
        return this;
    }
    
    /// <summary>
    /// Sets the request body as HTML content.
    /// </summary>
    /// <param name="html">The HTML content.</param>
    /// <param name="encoding">Optional encoding. Defaults to UTF-8.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithHtmlContent(string html, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(html);
        encoding ??= Encoding.UTF8;
        
        this.WithRawBody(html, encoding);
        this.WithHeader("Content-Type", $"text/html; charset={encoding.WebName}");
        
        return this;
    }
    
    
    public Request WithRawBody(RequestRawBodyBuilder bodyBuilder)
    {
        this._requestBuilder.WithRaw(bodyBuilder);
        return this;
    }

    public Request WithRawBody(string content, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        RequestRawBodyBuilder builder = new RequestRawBodyBuilder();
        builder.WithText(content, encoding);
        return this.WithRawBody(builder);
    }

    public Request WithJsonBody(string jsonContent, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        RequestRawBodyBuilder builder = new RequestRawBodyBuilder();
        builder.WithJson(jsonContent, encoding);
        return this.WithRawBody(builder);
    }
    
    public Request WithJsonBody(string jsonContent) => this.WithJsonBody(jsonContent, Encoding.UTF8);

    public Request WithJsonBody<T>(T obj)
    {
        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return this.WithJsonBody(json, Encoding.UTF8);
    }

    public Request WithBufferBody(byte[] buffer)
    {
        RequestRawBodyBuilder builder = new RequestRawBodyBuilder();
        builder.WithBuffer(buffer);
        return this.WithRawBody(builder);
    }

    public Request WithBufferBody(Memory<byte> buffer) => this.WithBufferBody(buffer.ToArray());

    public Request WithEncodedForm(RequestEncodedKeyValueListBodyBuilder formBuilder)
    {
        this._requestBuilder.WithEncodedForm(formBuilder);
        return this;
    }

    public Request WithEncodedForm(params (string key, string value)[] formData)
    {
        RequestEncodedKeyValueListBodyBuilder builder = new RequestEncodedKeyValueListBodyBuilder();
        foreach ((string key, string value) in formData)
            builder.AddText(key, value);
        return this.WithEncodedForm(builder);
    }

    public Request WithForm(RequestKeyValueListBodyBuilder formBuilder)
    {
        this._requestBuilder.WithForm(formBuilder);
        this._formBuilder = formBuilder;
        return this;
    }

    #endregion
    
    #region Content Type Helpers
    
    /// <summary>
    /// Gets the current Content-Type header value.
    /// </summary>
    /// <returns>The Content-Type header value or null if not set.</returns>
    public string? GetContentType()
    {
        return this._requestBuilder.RequestHeaderBuilder?.GetHeader("Content-Type");
    }
    
    /// <summary>
    /// Checks if the request has content.
    /// </summary>
    /// <returns>True if the request has a body, false otherwise.</returns>
    public bool HasContent()
    {
        return !this.Body.IsEmpty;
    }
    
    /// <summary>
    /// Gets the content length.
    /// </summary>
    /// <returns>The length of the request body in bytes.</returns>
    public int GetContentLength()
    {
        return this.Body.Length;
    }
    
    #endregion
}
