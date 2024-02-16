using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Objects;

namespace DevBase.Requests.Preparation.Header.Body;

public class RequestFormHeaderBuilder : HttpFormBuilder<RequestFormHeaderBuilder, string, object>
{
    public RequestFormHeaderBuilder()
    {
    }

    #region MimeFile Content

    public RequestFormHeaderBuilder AddFile(AFileObject fileObject) => AddFile(MimeFileObject.FromAFileObject(fileObject));
    public RequestFormHeaderBuilder AddFile(FileInfo fileInfo) => AddFile(MimeFileObject.FromFile(fileInfo));
    public RequestFormHeaderBuilder AddFile(string filePath) => AddFile(MimeFileObject.FromFile(filePath));
    
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
    
    // Idk why someone would use this but i'll add it anyways
    public RequestFormHeaderBuilder Remove(MimeFileObject mimeFileObject)
    {
        RemoveFormElement(mimeFileObject);
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
            
            if (!(value is MimeFileObject || value is string))
                throw new ElementValidationException(EnumValidationReason.DataMismatch);
            
            if (value is MimeFileObject mimeFileContent)
                AddFile(fieldName, mimeFileContent);

            if (value is string textContent)
                AddText(fieldName, textContent);
        }
    }
    
    protected override Action BuildAction { get; }
}