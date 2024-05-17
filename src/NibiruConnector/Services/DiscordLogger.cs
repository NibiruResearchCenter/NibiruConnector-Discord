using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NibiruConnector.Extensions;
using NibiruConnector.Options;
using Serilog.Events;

namespace NibiruConnector.Services;

public class DiscordLogger
{
    private readonly IMessageChannel _channel;

    public DiscordLogger(IOptions<DiscordOptions> options, DiscordSocketClient discordSocketClient)
    {
        _channel = discordSocketClient.GetMessageChannel(options.Value.ManagementChannel).GetAwaiter().GetResult();
    }

    public async Task Log(string message, LogEventLevel level = LogEventLevel.Information)
    {
        await _channel.Log(message, level);
    }
}
