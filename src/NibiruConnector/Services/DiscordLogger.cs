using Discord;
using NibiruConnector.Extensions;
using Serilog.Events;

namespace NibiruConnector.Services;

public class DiscordLogger
{
    private readonly IMessageChannel _channel;

    public DiscordLogger(
        [FromKeyedServices(Configuration.ManagementChannelKey)]
        IMessageChannel channel)
    {
        _channel = channel;
    }

    public async Task Log(string message, LogEventLevel level = LogEventLevel.Information)
    {
        var time = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        await _channel.SendMessageAsync(
            $"`[{time}] [{level.ToShort()}]` {message}");
    }
}
