using NibiruConnector.Options.Discord;

namespace NibiruConnector.Options;

public record DiscordOptions
{
    public string BotToken { get; set; } = string.Empty;

    public ulong GuildId { get; set; }

    public ulong ManagementChannel { get; set; }

    public List<DiscordChannelOptions> NotificationChannels { get; set; } = [];
}
