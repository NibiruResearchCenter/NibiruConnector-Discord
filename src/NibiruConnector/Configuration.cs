namespace NibiruConnector;

public static class Configuration
{
    public static readonly string ConfigurationFilePath = Environment
        .GetEnvironmentVariable("CONFIG_FILE") ?? "appsettings.yaml";

    public static IConfiguration Instance { get; set; } = null!;

    public static readonly string RuntimeEnvironment = Environment
        .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() ?? "production";

    public static readonly string HttpsProxy = Environment
        .GetEnvironmentVariable("HTTPS_PROXY") ?? string.Empty;

    public static readonly string NibiruConnectorVersion =
        typeof(Configuration).Assembly.GetName().Version?.ToString(3)
        ?? "0.0.0";
}
