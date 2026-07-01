namespace DevBase.IntegrationTests.Infrastructure;

internal static class LiveApiTestSettings
{
    public const string LiveApiSwitch = "DEVBASE_API_INTEGRATION_LIVE";
    public const string LegacyLiveApiSwitch = "DEVBASE_LIVE_API_TESTS";

    private static readonly string[] EnabledValues = ["1", "true", "yes", "on"];

    public static void RequireLiveApiTests()
    {
        if (IsEnabled(LiveApiSwitch) || IsEnabled(LegacyLiveApiSwitch)) {
            return;
        }

        Assert.Ignore($"Set {LiveApiSwitch}=1 to run live API integration tests.");
    }

    public static string RequireEnvironmentValue(string name)
    {
        string? value = GetEnvironmentValue(name);

        if (!string.IsNullOrWhiteSpace(value)) {
            return value;
        }

        Assert.Ignore($"Set {name} to run this credentialed live API integration test.");
        return string.Empty;
    }

    public static string? GetEnvironmentValue(string name)
    {
        string? value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static CancellationTokenSource CreateTimeout(TimeSpan? timeout = null)
    {
        return new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(45));
    }

    public static void IgnoreProviderUnavailable(string provider, System.Exception exception)
    {
        Assert.Ignore($"{provider} live API is not currently available: {exception.GetType().Name}: {exception.Message}");
    }

    public static void IgnoreProviderUnavailable(string provider, string reason)
    {
        Assert.Ignore($"{provider} live API is not currently available: {reason}");
    }

    private static bool IsEnabled(string name)
    {
        string? value = GetEnvironmentValue(name);
        return value is not null && EnabledValues.Contains(value, StringComparer.OrdinalIgnoreCase);
    }
}
