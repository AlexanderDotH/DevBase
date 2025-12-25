using DevBase.IO;

namespace DevBase.Format;

/// <summary>
/// Provides high-level parsing functionality using a specific file format.
/// </summary>
/// <typeparam name="P">The specific file format implementation.</typeparam>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public class FileParser<P, T> where P : FileFormat<string, T>
{
    /// <summary>
    /// Parses content from a string.
    /// </summary>
    /// <param name="content">The string content to parse.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromString(string content)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        return fileFormat.Parse(content);
    }
    
    /// <summary>
    /// Attempts to parse content from a string.
    /// </summary>
    /// <param name="content">The string content to parse.</param>
    /// <param name="parsed">The parsed object, or default on failure.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public bool TryParseFromString(string content, out T parsed)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        return fileFormat.TryParse(content, out parsed);
    }
    
    /// <summary>
    /// Parses content from a file on disk.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromDisk(string filePath)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        
        AFileObject file = AFile.ReadFileToObject(filePath);
        
        return fileFormat.Parse(file.ToStringData());
    }

    /// <summary>
    /// Attempts to parse content from a file on disk.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="parsed">The parsed object, or default on failure.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public bool TryParseFromDisk(string filePath, out T parsed)
    {
        P fileFormat = (P)Activator.CreateInstance(typeof(P));
        
        AFileObject file = AFile.ReadFileToObject(filePath);
        
        return fileFormat.TryParse(file.ToStringData(), out parsed);
    }

    /// <summary>
    /// Parses content from a file on disk using a FileInfo object.
    /// </summary>
    /// <param name="fileInfo">The FileInfo object representing the file.</param>
    /// <returns>The parsed object.</returns>
    public T ParseFromDisk(FileInfo fileInfo) => ParseFromDisk(fileInfo.FullName);
}