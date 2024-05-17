using Discord;
using Discord.WebSocket;

namespace NibiruConnector.Extensions;

public static class DiscordExtensions
{
    private static readonly Dictionary<ulong, IMessageChannel> MessageChannelInstanceCache = new();

    public static async Task<IMessageChannel> GetMessageChannel(this DiscordSocketClient client, ulong channelId)
    {
        var hasChannel = MessageChannelInstanceCache.TryGetValue(channelId, out var channel);
        if (hasChannel)
        {
            if (channel is not null)
            {
                return channel;
            }

            MessageChannelInstanceCache.Remove(channelId);
        }

        channel = (IMessageChannel)await client.GetChannelAsync(channelId);
        MessageChannelInstanceCache.Add(channelId, channel);

        return channel;
    }

    public static async Task<IReadOnlyList<IMessageChannel>> GetMessageChannels(this DiscordSocketClient client, params ulong[] channelIds)
    {
        var channels = new List<IMessageChannel>();
        foreach (var channelId in channelIds)
        {
            var channel = await client.GetMessageChannel(channelId);
            channels.Add(channel);
        }

        return channels;
    }
}
