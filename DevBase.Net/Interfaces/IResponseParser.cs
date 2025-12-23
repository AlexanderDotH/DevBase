namespace DevBase.Net.Interfaces;

public interface IResponseParser<T>
{
    Task<T> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
    bool CanParse(string? contentType);
}
