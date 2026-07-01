namespace DevBase.Api.Apis.Musixmatch;

public sealed class MusixMatchEmptyResponseDataException : InvalidOperationException
{
    public MusixMatchEmptyResponseDataException(string resourceName, string? resourceId = null)
        : base(CreateMessage(resourceName, resourceId))
    {
        this.ResourceName = resourceName;
        this.ResourceId = resourceId;
    }

    public string ResourceName { get; }

    public string? ResourceId { get; }

    private static string CreateMessage(string resourceName, string? resourceId)
    {
        return string.IsNullOrWhiteSpace(resourceId)
            ? $"Musixmatch response did not contain {resourceName} data."
            : $"Musixmatch response did not contain {resourceName} data for id {resourceId}.";
    }
}
