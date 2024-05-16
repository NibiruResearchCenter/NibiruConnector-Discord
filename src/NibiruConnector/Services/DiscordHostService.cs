using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NibiruConnector.Extensions;

namespace NibiruConnector.Services;

public class DiscordHostService : IHostedService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<DiscordHostService> _logger;

    public DiscordHostService(DiscordSocketClient discordSocketClient, ILogger<DiscordHostService> logger)
    {
        _discordSocketClient = discordSocketClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordSocketClient.LoginAsync(TokenType.Bot, Configuration.DiscordBotToken);

        _discordSocketClient.Log += msg =>
        {
            _logger.Log(msg.Severity.MapToLogLevel(), msg.Exception,
                "{Source} - {Message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        };

        await _discordSocketClient.StartAsync();
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordSocketClient.StopAsync();
    }
}
