using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Objects;

namespace DevBase.Requests.Preparation.Header.Body;

public class HttpFormHeaderBuilder : HttpFormBuilder<HttpFormHeaderBuilder, string, object>
{
    public HttpFormHeaderBuilder()
    {
    }

    public HttpFormHeaderBuilder AddFile(AFileObject fileObject) => AddFile(MimeFileObject.FromAFileObject(fileObject));
    public HttpFormHeaderBuilder AddFile(FileInfo fileInfo) => AddFile(MimeFileObject.FromFile(fileInfo));
    public HttpFormHeaderBuilder AddFile(string filePath) => AddFile(MimeFileObject.FromFile(filePath));
    
    public HttpFormHeaderBuilder AddFile(string fieldName, AFileObject fileObject) => AddFile(fieldName, MimeFileObject.FromAFileObject(fileObject));
    public HttpFormHeaderBuilder AddFile(string fieldName, FileInfo fileInfo) => AddFile(fieldName, MimeFileObject.FromFile(fileInfo));
    public HttpFormHeaderBuilder AddFile(string fieldName, string filePath) => AddFile(fieldName, MimeFileObject.FromFile(filePath));

    public HttpFormHeaderBuilder AddFile(MimeFileObject mimeFile) => AddFile(mimeFile, out string fieldName);
    
    private HttpFormHeaderBuilder AddFile(MimeFileObject mimeFile, out string fieldName)
    {
        fieldName = mimeFile.FileInfo.Name;
        AddFile(fieldName, mimeFile);
        return this;
    }
    
    public HttpFormHeaderBuilder AddFile(string fieldName, MimeFileObject mimeFile)
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