using System.ComponentModel.DataAnnotations;
using System.Text;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Web.RequestData.Data;
using DevBase.Web.RequestData.Data.Mime;

namespace DevBase.Web.RequestData.Types;

public class MultipartFormHolder
{
    private readonly AList<MultipartElement> _multipartElements;
    private string _boundaryContentType;

    public MultipartFormHolder()
    {
        this._multipartElements = new AList<MultipartElement>();
    }

    public void AddElement(MultipartElement element) => 
        this._multipartElements.Add(element);
    
    public byte[] GenerateData()
    {
        AList<byte> data = new AList<byte>();

        string boundary = string.Format("---------------------------{0}", 
            DateTime.Now.Ticks.ToString("x"));

        this._boundaryContentType = string.Format("boundary={0}", boundary);
        
        byte[] boundaryData = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}\r\n", boundary));

        // Write normal formdata
        for (int i = 0; i < this._multipartElements.Length; i++)
        {
            MultipartElement element = this._multipartElements.Get(i);

            if (element.Data is string)
            {
                string formatedElement = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
                    element.Key, element.Data);

                data.AddRange(boundaryData);
                data.AddRange(Encoding.UTF8.GetBytes(formatedElement));
            }
        }

        // Write file formdata
        for (int i = 0; i < this._multipartElements.Length; i++)
        {
            MultipartElement element = this._multipartElements.Get(i);

            if (element.Data is byte[] rawBytes)
            {
                string formatedElement = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    element.Key, 
                    "file.mp3", 
                    MimeTypeMap.GetMimeType("mp3"));
                
                data.AddRange(boundaryData);
                data.AddRange(Encoding.UTF8.GetBytes(formatedElement));
                data.AddRange(rawBytes);
            }
            
            if (element.Data is AFileObject fileObject)
            {
                string formatedElement = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    element.Key, 
                    fileObject.FileInfo.Name, 
                    MimeTypeMap.GetMimeType(fileObject.FileInfo.Extension));
                
                data.AddRange(boundaryData);
                data.AddRange(Encoding.UTF8.GetBytes(formatedElement));
                data.AddRange(fileObject.Buffer.ToArray());
            }
        }
        
        byte[] tail = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));
        data.AddRange(tail);

        return data.GetAsArray();
    }

    public string BoundaryContentType
    {
        get => _boundaryContentType;
    }
}