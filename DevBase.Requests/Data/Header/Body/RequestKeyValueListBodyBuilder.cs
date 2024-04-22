using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Objects;
using DevBase.Requests.Struct;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Data.Header.Body;

public class RequestKeyValueListBodyBuilder : HttpKeyValueListBuilder<RequestKeyValueListBodyBuilder, string, object>
{
    public Memory<byte> Bounds { get; private set; }
    public Memory<byte> Separator { get; private set; }
    public Memory<byte> Tail { get; private set; }
    
    public RequestKeyValueListBodyBuilder() 
    {
        ContentDispositionBounds bounds = ContentDispositionUtils.GetBounds();

        Bounds = bounds.Bounds;
        Separator = bounds.Separator;
        Tail = bounds.Tail;
    }

    protected override Action BuildAction => () =>
    {
        List<Memory<byte>> buffer = new List<Memory<byte>>();
        
        for (var i = 0; i < Entries.Count; i++)
        {
            KeyValuePair<string, object> formEntry = Entries[i];

            if (!(formEntry.Value is string || formEntry.Value is MimeFileObject))
                continue;
            
            buffer.Add(ContentDispositionUtils.NewLine);
            buffer.Add(Separator);
            buffer.Add(ContentDispositionUtils.NewLine);
            
            if (formEntry.Value is MimeFileObject mimeEntry) 
                buffer.Add(ContentDispositionUtils.FromFile(formEntry.Key, mimeEntry));

            if (formEntry.Value is string textEntry)
                buffer.Add(ContentDispositionUtils.FromValue(formEntry.Key, textEntry));
        }
 
        buffer.Add(ContentDispositionUtils.NewLine);
        buffer.Add(Tail);

        Buffer = BufferUtils.Combine(buffer);
    };
    
    #region MimeFile Content
    public RequestKeyValueListBodyBuilder AddFile(byte[] buffer) => AddFile(AFileObject.FromBuffer(buffer));
    public RequestKeyValueListBodyBuilder AddFile(AFileObject fileObject) => AddFile(MimeFileObject.FromAFileObject(fileObject));
    public RequestKeyValueListBodyBuilder AddFile(FileInfo fileInfo) => AddFile(MimeFileObject.FromFile(fileInfo));
    public RequestKeyValueListBodyBuilder AddFile(string filePath) => AddFile(MimeFileObject.FromFile(filePath));
    
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, byte[] buffer) => AddFile(fieldName, AFileObject.FromBuffer(buffer));
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, AFileObject fileObject) => AddFile(fieldName, MimeFileObject.FromAFileObject(fileObject));
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, FileInfo fileInfo) => AddFile(fieldName, MimeFileObject.FromFile(fileInfo));
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, string filePath) => AddFile(fieldName, MimeFileObject.FromFile(filePath));

    public RequestKeyValueListBodyBuilder AddFile(MimeFileObject mimeFile) => AddFile(mimeFile, out string fieldName);
    
    private RequestKeyValueListBodyBuilder AddFile(MimeFileObject mimeFile, out string fieldName)
    {
        fieldName = mimeFile.FileInfo.Name;
        AddFile(fieldName, mimeFile);
        return this;
    }
    
    public RequestKeyValueListBodyBuilder AddFile(string fieldName, MimeFileObject mimeFile)
    {
        AddEntry(fieldName, mimeFile);
        return this;
    }
    
    #endregion

    #region Text Content

    public RequestKeyValueListBodyBuilder AddText(string key, string value)
    {
        AddEntry(key, value);
        return this;
    }

    #endregion

    #region Fields

    public RequestKeyValueListBodyBuilder RemoveEntryAt(int index)
    {
        RemoveEntry(index);
        return this;
    }

    public RequestKeyValueListBodyBuilder Remove(string fieldName)
    {
        RemoveEntryKey(fieldName);
        return this;
    }
    
    public object this[string fieldName]
    {
        set
        {
            if (value is null)
            {
                RemoveEntryKey(fieldName);
                return;
            }
            
            if (!(value is MimeFileObject || value is string || value is byte[]))
                throw new ElementValidationException(EnumValidationReason.DataMismatch);

            if (value is byte[] buffer)
                AddFile(fieldName, buffer);
            
            if (value is MimeFileObject mimeFileContent)
                AddFile(fieldName, mimeFileContent);

            if (value is string textContent)
                AddText(fieldName, textContent);
        }
    }

    #endregion
}