using System.Diagnostics.CodeAnalysis;
using Discord;
using MassTransit;
using NibiruConnector.Contracts;
using NibiruConnector.Exceptions;
using NibiruConnector.Extensions;
using NibiruConnector.Services;
using Serilog.Events;

namespace NibiruConnector.Consumers;

public class UptimeKumaDiscordNotification : IConsumer<UptimeKumaStatusUpdate>
{
    private readonly IMessageChannel _notificationChannel;
    private readonly DiscordLogger _discordLogger;
    private readonly ILogger<UptimeKumaDiscordNotification> _logger;

    public UptimeKumaDiscordNotification(
        [FromKeyedServices(Configuration.NotificationChannelKey)]
        IMessageChannel notificationChannel,
        DiscordLogger discordLogger,
        ILogger<UptimeKumaDiscordNotification> logger)
    {
        _notificationChannel = notificationChannel;
        _discordLogger = discordLogger;
        _logger = logger;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public async Task Consume(ConsumeContext<UptimeKumaStatusUpdate> context)
    {
        var data = context.Message.Data;
        _logger.LogDebug("Received Uptime Kuma status update: {Message}", data.Message);

        try
        {
            var embed = data.GetStatusChangedEmbed();
            await _notificationChannel.SendMessageAsync(embed: embed);
        }
        catch (UnknownStatusException e)
        {
            await _discordLogger.Log($"UnknownStatusException: {e.Message}", LogEventLevel.Warning);
        }
        catch (Exception e)
        {
            await _discordLogger.Log($"Exception: {e.Message}", LogEventLevel.Error);
        }
    }
}
