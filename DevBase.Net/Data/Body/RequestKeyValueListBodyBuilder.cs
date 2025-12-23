using DevBase.IO;
using DevBase.Net.Abstract;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;
using DevBase.Net.Objects;
using DevBase.Net.Struct;
using DevBase.Net.Utils;

namespace DevBase.Net.Data.Body;

public class RequestKeyValueListBodyBuilder : HttpKeyValueListBuilder<RequestKeyValueListBodyBuilder, string, object>
{
    public Memory<byte> Bounds { get; private set; }
    public Memory<byte> Separator { get; private set; }
    public Memory<byte> Tail { get; private set; }
    
    public RequestKeyValueListBodyBuilder() 
    {
        ContentDispositionBounds bounds = ContentDispositionUtils.GetBounds();

        this.Bounds = bounds.Bounds;
        this.Separator = bounds.Separator;
        this.Tail = bounds.Tail;
    }

    protected override Action BuildAction => () =>
    {
        List<Memory<byte>> buffer = new List<Memory<byte>>();
        
        for (int i = 0; i < this.Entries.Count; i++)
        {
            KeyValuePair<string, object> formEntry = this.Entries[i];

            if (!(formEntry.Value is string || formEntry.Value is MimeFileObject))
                continue;
            
            buffer.Add(ContentDispositionUtils.NewLine);
            buffer.Add(this.Separator);
            buffer.Add(ContentDispositionUtils.NewLine);
            
            if (formEntry.Value is MimeFileObject mimeEntry) 
                buffer.Add(ContentDispositionUtils.FromFile(formEntry.Key, mimeEntry));

            if (formEntry.Value is string textEntry)
                buffer.Add(ContentDispositionUtils.FromValue(formEntry.Key, textEntry));
        }
 
        buffer.Add(ContentDispositionUtils.NewLine);
        buffer.Add(this.Tail);

        this.Buffer = BufferUtils.Combine(buffer);
    };
    
    public RequestKeyValueListBodyBuilder AddFile(byte[] buffer) => this.AddFile(AFileObject.FromBuffer(buffer));
    public RequestKeyValueListBodyBuilder AddFile(AFileObject fileObject) => this.AddFile(MimeFileObject.FromAFileObject(fileObject));
    public RequestKeyValueListBodyBuilder AddFile(FileInfo fileInfo) => this.AddFile(MimeFileObject.FromFile(fileInfo));
    public RequestKeyValueListBodyBuilder AddFile(string filePath) => this.AddFile(MimeFileObject.FromFile(filePath));
    
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, byte[] buffer) => this.AddFile(fieldName, AFileObject.FromBuffer(buffer));
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, AFileObject fileObject) => this.AddFile(fieldName, MimeFileObject.FromAFileObject(fileObject));
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, FileInfo fileInfo) => this.AddFile(fieldName, MimeFileObject.FromFile(fileInfo));
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, string filePath) => this.AddFile(fieldName, MimeFileObject.FromFile(filePath));

    public RequestKeyValueListBodyBuilder AddFile(MimeFileObject mimeFile) => this.AddFile(mimeFile, out string fieldName);
    
    private RequestKeyValueListBodyBuilder AddFile(MimeFileObject mimeFile, out string fieldName)
    {
        fieldName = mimeFile.FileInfo.Name;
        this.AddFile(fieldName, mimeFile);
        return this;
    }
    
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, MimeFileObject mimeFile)
    {
        this.AddEntry(fieldName, mimeFile);
        return this;
    }
    
    public RequestKeyValueListBodyBuilder AddText(string key, string value)
    {
        this.AddEntry(key, value);
        return this;
    }

    public RequestKeyValueListBodyBuilder RemoveEntryAt(int index)
    {
        this.RemoveEntry(index);
        return this;
    }

    public RequestKeyValueListBodyBuilder Remove(string fieldName)
    {
        this.RemoveEntryKey(fieldName);
        return this;
    }
    
    public object this[string fieldName]
    {
        set
        {
            if (value is null)
            {
                this.RemoveEntryKey(fieldName);
                return;
            }
            
            if (!(value is MimeFileObject || value is string || value is byte[]))
                throw new ElementValidationException(EnumValidationReason.DataMismatch);

            if (value is byte[] buffer)
                this.AddFile(fieldName, buffer);
            
            if (value is MimeFileObject mimeFileContent)
                this.AddFile(fieldName, mimeFileContent);

            if (value is string textContent)
                this.AddText(fieldName, textContent);
        }
    }
}