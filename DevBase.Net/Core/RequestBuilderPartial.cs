using System.Text;
using DevBase.IO;
using DevBase.Net.Data.Body;
using DevBase.Net.Data.Body.Mime;
using DevBase.Net.Objects;

namespace DevBase.Net.Core;

/// <summary>
/// Partial class for Request providing fluent file upload builder methods.
/// Simplifies building multipart/form-data requests with files and form fields.
/// </summary>
public partial class Request
{
    #region Fluent Form Builder
    
    /// <summary>
    /// Starts building a multipart form request with the specified form builder action.
    /// </summary>
    /// <param name="builderAction">Action to configure the form builder.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithMultipartForm(Action<MultipartFormBuilder> builderAction)
    {
        ArgumentNullException.ThrowIfNull(builderAction);
        
        MultipartFormBuilder builder = new MultipartFormBuilder();
        builderAction(builder);
        return this.WithForm(builder.Build());
    }
    
    /// <summary>
    /// Creates a file upload request with a single file.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithSingleFileUpload(string fieldName, string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        
        return this.WithMultipartForm(form => form.AddFile(fieldName, filePath));
    }
    
    /// <summary>
    /// Creates a file upload request with a single file and additional form fields.
    /// </summary>
    /// <param name="fieldName">The form field name for the file.</param>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="additionalFields">Additional form fields.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithSingleFileUpload(string fieldName, string filePath, params (string name, string value)[] additionalFields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        
        return this.WithMultipartForm(form =>
        {
            form.AddFile(fieldName, filePath);
            foreach ((string name, string value) in additionalFields)
            {
                form.AddField(name, value);
            }
        });
    }
    
    /// <summary>
    /// Creates a file upload request with multiple files.
    /// </summary>
    /// <param name="fieldName">The form field name (same for all files).</param>
    /// <param name="filePaths">Paths to the files.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithMultipleFileUpload(string fieldName, params string[] filePaths)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(filePaths);
        
        return this.WithMultipartForm(form =>
        {
            foreach (string path in filePaths)
            {
                form.AddFile(fieldName, path);
            }
        });
    }
    
    /// <summary>
    /// Creates a file upload request with multiple files, each with its own field name.
    /// </summary>
    /// <param name="files">Array of field name and file path pairs.</param>
    /// <returns>The request instance for method chaining.</returns>
    public Request WithMultipleFileUpload(params (string fieldName, string filePath)[] files)
    {
        ArgumentNullException.ThrowIfNull(files);
        
        return this.WithMultipartForm(form =>
        {
            foreach ((string fieldName, string filePath) in files)
            {
                form.AddFile(fieldName, filePath);
            }
        });
    }
    
    #endregion
}

/// <summary>
/// Fluent builder for constructing multipart form data requests.
/// Supports files, text fields, and binary data.
/// </summary>
public class MultipartFormBuilder
{
    private readonly RequestKeyValueListBodyBuilder _builder;
    
    public MultipartFormBuilder()
    {
        this._builder = new RequestKeyValueListBodyBuilder();
    }
    
    /// <summary>
    /// Gets the boundary string for this multipart form.
    /// </summary>
    public string BoundaryString => this._builder.BoundaryString;
    
    #region File Operations
    
    /// <summary>
    /// Adds a file from a file path.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        
        FileInfo fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
            throw new FileNotFoundException("File not found", filePath);
        
        Memory<byte> buffer = AFile.ReadFile(fileInfo);
        AFileObject fileObject = AFileObject.FromBuffer(buffer.ToArray(), fileInfo.Name);
        MimeFileObject mimeFile = MimeFileObject.FromAFileObject(fileObject);
        this._builder.AddFile(fieldName, mimeFile);
        
        return this;
    }
    
    /// <summary>
    /// Adds a file from a FileInfo object.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="fileInfo">The file information.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, FileInfo fileInfo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(fileInfo);
        
        if (!fileInfo.Exists)
            throw new FileNotFoundException("File not found", fileInfo.FullName);
        
        Memory<byte> buffer = AFile.ReadFile(fileInfo);
        AFileObject fileObject = AFileObject.FromBuffer(buffer.ToArray(), fileInfo.Name);
        MimeFileObject mimeFile = MimeFileObject.FromAFileObject(fileObject);
        this._builder.AddFile(fieldName, mimeFile);
        
        return this;
    }
    
    /// <summary>
    /// Adds a file from an AFileObject.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="fileObject">The AFileObject containing file data.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, AFileObject fileObject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(fileObject);
        
        MimeFileObject mimeFile = MimeFileObject.FromAFileObject(fileObject);
        this._builder.AddFile(fieldName, mimeFile);
        
        return this;
    }
    
    /// <summary>
    /// Adds a file from a MimeFileObject.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="mimeFile">The MimeFileObject containing file data and type.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, MimeFileObject mimeFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(mimeFile);
        
        this._builder.AddFile(fieldName, mimeFile);
        return this;
    }
    
    /// <summary>
    /// Adds a file from a byte array with a filename.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="data">The file data.</param>
    /// <param name="filename">Optional filename. Defaults to fieldName.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, byte[] data, string? filename = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(data);
        
        filename ??= fieldName;
        AFileObject fileObject = AFileObject.FromBuffer(data, filename);
        MimeFileObject mimeFile = MimeFileObject.FromAFileObject(fileObject);
        this._builder.AddFile(fieldName, mimeFile);
        
        return this;
    }
    
    /// <summary>
    /// Adds a file from a Memory buffer with a filename.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="data">The file data.</param>
    /// <param name="filename">Optional filename. Defaults to fieldName.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, Memory<byte> data, string? filename = null)
    {
        return this.AddFile(fieldName, data.ToArray(), filename);
    }
    
    /// <summary>
    /// Adds a file from a Stream.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="stream">The stream containing file data.</param>
    /// <param name="filename">The filename.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFile(string fieldName, Stream stream, string filename)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);
        
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return this.AddFile(fieldName, ms.ToArray(), filename);
    }
    
    #endregion
    
    #region Text Field Operations
    
    /// <summary>
    /// Adds a text form field.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="value">The field value.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddField(string fieldName, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        
        this._builder.AddText(fieldName, value ?? string.Empty);
        return this;
    }
    
    /// <summary>
    /// Adds multiple text form fields.
    /// </summary>
    /// <param name="fields">Array of field name and value pairs.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFields(params (string name, string value)[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        
        foreach ((string name, string value) in fields)
        {
            this.AddField(name, value);
        }
        return this;
    }
    
    /// <summary>
    /// Adds form fields from a dictionary.
    /// </summary>
    /// <param name="fields">Dictionary of field names and values.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddFields(IDictionary<string, string> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        
        foreach (KeyValuePair<string, string> field in fields)
        {
            this.AddField(field.Key, field.Value);
        }
        return this;
    }
    
    /// <summary>
    /// Adds a form field with a typed value (converts to string).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="value">The field value.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddField<T>(string fieldName, T value)
    {
        return this.AddField(fieldName, value?.ToString() ?? string.Empty);
    }
    
    #endregion
    
    #region Binary Data Operations
    
    /// <summary>
    /// Adds binary data as a form field (without file semantics).
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="data">The binary data.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder AddBinaryData(string fieldName, byte[] data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(data);
        
        this._builder.AddFile(fieldName, data);
        return this;
    }
    
    #endregion
    
    #region Builder Operations
    
    /// <summary>
    /// Removes a field by name.
    /// </summary>
    /// <param name="fieldName">The field name to remove.</param>
    /// <returns>The builder for method chaining.</returns>
    public MultipartFormBuilder RemoveField(string fieldName)
    {
        this._builder.Remove(fieldName);
        return this;
    }
    
    /// <summary>
    /// Gets the number of entries in the form.
    /// </summary>
    public int Count => this._builder.GetEntries().Count();
    
    /// <summary>
    /// Builds and returns the underlying RequestKeyValueListBodyBuilder.
    /// </summary>
    /// <returns>The configured form builder.</returns>
    public RequestKeyValueListBodyBuilder Build()
    {
        return this._builder;
    }
    
    #endregion
    
    #region Static Factory Methods
    
    /// <summary>
    /// Creates a MultipartFormBuilder from a single file.
    /// </summary>
    /// <param name="fieldName">The form field name.</param>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>A new MultipartFormBuilder with the file added.</returns>
    public static MultipartFormBuilder FromFile(string fieldName, string filePath)
    {
        return new MultipartFormBuilder().AddFile(fieldName, filePath);
    }
    
    /// <summary>
    /// Creates a MultipartFormBuilder from multiple files.
    /// </summary>
    /// <param name="files">Array of field name and file path pairs.</param>
    /// <returns>A new MultipartFormBuilder with the files added.</returns>
    public static MultipartFormBuilder FromFiles(params (string fieldName, string filePath)[] files)
    {
        MultipartFormBuilder builder = new MultipartFormBuilder();
        foreach ((string fieldName, string filePath) in files)
        {
            builder.AddFile(fieldName, filePath);
        }
        return builder;
    }
    
    /// <summary>
    /// Creates a MultipartFormBuilder from form fields.
    /// </summary>
    /// <param name="fields">Array of field name and value pairs.</param>
    /// <returns>A new MultipartFormBuilder with the fields added.</returns>
    public static MultipartFormBuilder FromFields(params (string name, string value)[] fields)
    {
        return new MultipartFormBuilder().AddFields(fields);
    }
    
    #endregion
}
