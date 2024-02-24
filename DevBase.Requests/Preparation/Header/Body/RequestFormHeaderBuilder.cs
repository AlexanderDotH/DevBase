using System.Text;
using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Objects;
using DevBase.Requests.Struct;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.Body;

public class RequestFormHeaderBuilder : HttpFormBuilder<RequestFormHeaderBuilder, string, object>
{
    public Memory<byte> Bounds { get; private set; }
    public Memory<byte> Separator { get; private set; }
    public Memory<byte> Tail { get; private set; }
    
    public RequestFormHeaderBuilder()
    {
        ContentDispositionBounds bounds = ContentDispositionUtils.GetBounds();

        Bounds = bounds.Bounds;
        Separator = bounds.Separator;
        Tail = bounds.Tail;
    }

    #region MimeFile Content
    public RequestFormHeaderBuilder AddFile(byte[] buffer) => AddFile(AFileObject.FromBuffer(buffer));
    public RequestFormHeaderBuilder AddFile(AFileObject fileObject) => AddFile(MimeFileObject.FromAFileObject(fileObject));
    public RequestFormHeaderBuilder AddFile(FileInfo fileInfo) => AddFile(MimeFileObject.FromFile(fileInfo));
    public RequestFormHeaderBuilder AddFile(string filePath) => AddFile(MimeFileObject.FromFile(filePath));
    
    public RequestFormHeaderBuilder AddFile(string fieldName, byte[] buffer) => AddFile(fieldName, AFileObject.FromBuffer(buffer));
    public RequestFormHeaderBuilder AddFile(string fieldName, AFileObject fileObject) => AddFile(fieldName, MimeFileObject.FromAFileObject(fileObject));
    public RequestFormHeaderBuilder AddFile(string fieldName, FileInfo fileInfo) => AddFile(fieldName, MimeFileObject.FromFile(fileInfo));
    public RequestFormHeaderBuilder AddFile(string fieldName, string filePath) => AddFile(fieldName, MimeFileObject.FromFile(filePath));

    public RequestFormHeaderBuilder AddFile(MimeFileObject mimeFile) => AddFile(mimeFile, out string fieldName);
    
    private RequestFormHeaderBuilder AddFile(MimeFileObject mimeFile, out string fieldName)
    {
        fieldName = mimeFile.FileInfo.Name;
        AddFile(fieldName, mimeFile);
        return this;
    }
    
    public RequestFormHeaderBuilder AddFile(string fieldName, MimeFileObject mimeFile)
    {
        AddFormElement(fieldName, mimeFile);
        return this;
    }
    
    #endregion

    #region Text Content

    public RequestFormHeaderBuilder AddText(string key, string value)
    {
        AddFormElement(key, value);
        return this;
    }

    #endregion

    #region Fields

    public RequestFormHeaderBuilder RemoveEntryAt(int index)
    {
        RemoveFormElement(index);
        return this;
    }

    public RequestFormHeaderBuilder Remove(string fieldName)
    {
        RemoveFormElement(fieldName);
        return this;
    }
    
    public object this[string fieldName]
    {
        set
        {
            if (value is null)
            {
                RemoveFormElement(fieldName);
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
    
    protected override Action BuildAction => () =>
    {
        
        List<Memory<byte>> buffer = new List<Memory<byte>>();
        
        for (var i = 0; i < FormData.Count; i++)
        {
            KeyValuePair<string, object> formEntry = FormData[i];

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
}