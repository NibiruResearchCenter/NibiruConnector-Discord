namespace NibiruConnector;

public static class Configuration
{
    public static readonly string ConfigurationFilePath = Environment
        .GetEnvironmentVariable("CONFIG_FILE") ?? "appsettings.yaml";

    public static IConfiguration Instance { get; set; } = null!;

    public static readonly string RuntimeEnvironment = Environment
        .GetEnvironmentVariable("RUNTIME_ENV")?.ToLowerInvariant() ?? "production";

    public static readonly string HttpsProxy = Environment
        .GetEnvironmentVariable("HTTPS_PROXY") ?? string.Empty;
}
