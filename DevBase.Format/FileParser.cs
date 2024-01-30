using DevBase.IO;

namespace DevBase.Format;

public class FileParser<P, T> where P : FileFormat<string, T>
{
    public T ParseFromString(string content)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        return fileFormat.Parse(content);
    }
    
    public bool TryParseFromString(string content, out T parsed)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        return fileFormat.TryParse(content, out parsed);
    }
    
    public T ParseFromDisk(string filePath)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        
        AFileObject file = AFile.ReadFileToObject(filePath);
        
        return fileFormat.Parse(file.ToStringData());
    }

    public bool TryParseFromDisk(string filePath, out T parsed)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        
        AFileObject file = AFile.ReadFileToObject(filePath);
        
        return fileFormat.TryParse(file.ToStringData(), out parsed);
    }

    public T ParseFromDisk(FileInfo fileInfo) => ParseFromDisk(fileInfo.FullName);
}