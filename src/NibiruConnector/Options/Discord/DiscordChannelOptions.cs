namespace NibiruConnector.Options.Discord;

public record DiscordChannelOptions
{
    public ulong Id { get; set; }

    public string Ref { get; set; } = string.Empty;
}
