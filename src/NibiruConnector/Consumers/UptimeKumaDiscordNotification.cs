using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.WebSocket;
using MassTransit;
using Microsoft.Extensions.Options;
using NibiruConnector.Contracts;
using NibiruConnector.Exceptions;
using NibiruConnector.Extensions;
using NibiruConnector.Options;
using NibiruConnector.Services;
using Serilog.Events;

namespace NibiruConnector.Consumers;

public class UptimeKumaDiscordNotification : IConsumer<UptimeKumaStatusUpdate>
{
    private readonly DiscordLogger _discordLogger;
    private readonly IOptions<DiscordOptions> _options;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<UptimeKumaDiscordNotification> _logger;

    public UptimeKumaDiscordNotification(
        DiscordLogger discordLogger,
        IOptions<DiscordOptions> options,
        DiscordSocketClient discordSocketClient,
        ILogger<UptimeKumaDiscordNotification> logger)
    {
        _discordLogger = discordLogger;
        _options = options;
        _discordSocketClient = discordSocketClient;
        _logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public async Task Consume(ConsumeContext<UptimeKumaStatusUpdate> context)
    {
        var data = context.Message.Data;
        _logger.LogDebug("Received Uptime Kuma status update: {Message}", data.Message);

        var channels = _options.Value.NotificationChannels
            .Where(x => context.Message.ChannelRefs.Contains(x.Ref))
            .ToList();
        var channelId = channels.Select(x => x.Id).ToArray();
        var messageChannels = await _discordSocketClient.GetMessageChannels(channelId);
        Embed embed;

        try
        {
            embed = data.GetStatusChangedEmbed();
        }
        catch (UnknownStatusException e)
        {
            _logger.LogWarning(e, "Unknown status from Uptime Kuma heartbeat");
            await _discordLogger.Log($"UnknownStatusException: {e.Message}", LogEventLevel.Warning);
            return;
        }

        foreach (var messageChannel in messageChannels)
        {
            try
            {
                await messageChannel.SendMessageAsync(embed: embed);
                var channelRef = channels.First(x => x.Id == messageChannel.Id).Ref;
                _logger.LogInformation("Sent Uptime Kuma status update to channel {ChannelRef}", channelRef);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown exception occurred while sending Uptime Kuma status update to Discord channel");
                await _discordLogger.Log($"Exception: {e.Message}", LogEventLevel.Error);
            }
        }
    }
}
