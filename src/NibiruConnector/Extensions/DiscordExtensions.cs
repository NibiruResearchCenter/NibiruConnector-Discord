using Discord;
using Discord.WebSocket;

namespace NibiruConnector.Extensions;

public static class DiscordExtensions
{
    public static async Task<IMessageChannel> GetMessageChannel(this DiscordSocketClient client, ulong channelId)
    {
        var channel = await client.GetChannelAsync(channelId);
        return (IMessageChannel)channel;
    }

    public static async Task<IMessageChannel> GetNotificationChannel(this DiscordSocketClient client)
    {
        return await client.GetMessageChannel(Configuration.DiscordNotificationChannelId);
    }

    public static async Task<IMessageChannel> GetManagementChannel(this DiscordSocketClient client)
    {
        return await client.GetMessageChannel(Configuration.DiscordManagementChannelId);
    }
}
