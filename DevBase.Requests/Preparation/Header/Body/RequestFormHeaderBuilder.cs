using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Objects;

namespace DevBase.Requests.Preparation.Header.Body;

public class RequestFormHeaderBuilder : HttpFormBuilder<RequestFormHeaderBuilder, string, object>
{
    public RequestFormHeaderBuilder()
    {
    }
    
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

    public MimeFileObject this[string fieldName]
    {
        set
        {
            AddFile(fieldName, value);
        }
    }
    
    protected override Action BuildAction { get; }
}