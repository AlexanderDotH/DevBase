using DevBase.IO;
using DevBase.Requests.Data.Header.Body.Mime;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Utilities;

namespace DevBase.Requests.Objects;

public class MimeFileObject : AFileObject
{
    public ReadOnlyMemory<char> MimeType { get; private set; }

    private static readonly MimeDictionary _mimeDictionary = new MimeDictionary();
    
    public MimeFileObject(FileInfo fileInfo, Memory<byte> buffer) : base(fileInfo, buffer)
    {
        if (fileInfo == null || buffer.IsEmpty)
            throw new ElementValidationException(EnumValidationReason.Empty);

        MimeType = _mimeDictionary.GetMimeTypeAsMemory(fileInfo.Extension);
    }

    public MimeFileObject(FileInfo fileInfo) : base(fileInfo, true)
    {
        if (!fileInfo.Exists)
            throw new FileNotFoundException();

        MimeType = _mimeDictionary.GetMimeTypeAsMemory(fileInfo.Extension);
    }

    public MimeFileObject(string filePath) : this(new FileInfo(filePath)) {}

    public static MimeFileObject FromFile(string filePath) => new MimeFileObject(filePath);
    public static MimeFileObject FromFile(FileInfo fileInfo) => new MimeFileObject(fileInfo);
    public static MimeFileObject FromBinary(FileInfo fileInfo, byte[] buffer) => new MimeFileObject(fileInfo, buffer);
    public static MimeFileObject FromBuffer(byte[] buffer) => new MimeFileObject(new FileInfo("buffer.bin"), buffer);
    public static MimeFileObject FromAFileObject(AFileObject fileObject) =>
        new MimeFileObject(fileObject.FileInfo, fileObject.Buffer);
}