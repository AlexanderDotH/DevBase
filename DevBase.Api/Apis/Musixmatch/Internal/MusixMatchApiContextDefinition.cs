namespace DevBase.Api.Apis.Musixmatch.Internal;

internal sealed record MusixMatchApiContextDefinition(
    string ApiUrl,
    string? AppId,
    bool RequiresSignature)
{
    private static readonly MusixMatchApiContextDefinition Desktop = new(
        "https://apic-desktop.musixmatch.com/ws/1.1/",
        "web-desktop-app-v1.0",
        true);

    private static readonly MusixMatchApiContextDefinition Web = new(
        "https://www.musixmatch.com/ws/1.1/",
        "community-app-v1.0",
        true);

    private static readonly MusixMatchApiContextDefinition Ios = new(
        "https://apic.musixmatch.com/ws/1.1/",
        "mac-ios-v2.0",
        true);

    private static readonly MusixMatchApiContextDefinition AuthWeb = new(
        "https://account.musixmatch.com/ws/1.1/",
        null,
        false);

    public static MusixMatchApiContextDefinition For(MusixMatchApiContext context)
    {
        return context switch
        {
            MusixMatchApiContext.Web => Web,
            MusixMatchApiContext.Ios => Ios,
            MusixMatchApiContext.AuthWeb => AuthWeb,
            _ => Desktop
        };
    }
}
