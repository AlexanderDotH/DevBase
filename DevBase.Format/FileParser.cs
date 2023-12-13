using DevBase.IO;

namespace DevBase.Format;

public class FileParser<P, T> where P : FileFormat<string, T>
{
    public T ParseFromString(string content)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        return fileFormat.Parse(content);
    }
    
    public T ParseFromDisk(string filePath)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        
        AFileObject file = AFile.ReadFile(filePath);
        
        return fileFormat.Parse(file.ToStringData());
    }

    public T ParseFromDisk(FileInfo fileInfo) => ParseFromDisk(fileInfo.FullName);
}