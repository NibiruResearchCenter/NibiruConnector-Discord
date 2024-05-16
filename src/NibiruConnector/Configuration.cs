namespace NibiruConnector;

public static class Configuration
{
    public static readonly string RuntimeEnvironment = Environment
        .GetEnvironmentVariable("DOTNET_ENV") ?? "Development";

    public static readonly string HttpsProxy = Environment
        .GetEnvironmentVariable("HTTPS_PROXY") ?? "";

    public static readonly string DiscordBotToken = Environment
        .GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? "";

    public static readonly string DiscordManagementChannel = Environment
        .GetEnvironmentVariable("DISCORD_MANAGEMENT_CHANNEL") ?? "";

    public static readonly string DiscordNotificationChannel = Environment
        .GetEnvironmentVariable("DISCORD_NOTIFICATION_CHANNEL") ?? "";

    public static readonly string LoggerFileOutput = Environment
        .GetEnvironmentVariable("LOGGER_FILE_OUTPUT") ?? "";

    public static readonly string LoggerLevel = Environment
        .GetEnvironmentVariable("LOGGER_LEVEL")
        ?? (RuntimeEnvironment == "Development" ? "Debug" : "Information");

    public static readonly ulong DiscordManagementChannelId = ulong.Parse(DiscordManagementChannel);
    public static readonly ulong DiscordNotificationChannelId = ulong.Parse(DiscordNotificationChannel);

    public const string ManagementChannelKey = "Management";
    public const string NotificationChannelKey = "Notification";
}
