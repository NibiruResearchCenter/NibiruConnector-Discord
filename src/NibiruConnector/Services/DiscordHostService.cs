using Discord;
using Discord.WebSocket;
using NibiruConnector.Extensions;

namespace NibiruConnector.Services;

public class DiscordHostService : IHostedService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<DiscordHostService> _logger;
    private IMessageChannel? _managementChannel;

    public DiscordHostService(
        DiscordSocketClient discordSocketClient,
        ILogger<DiscordHostService> logger)
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

        _managementChannel = await _discordSocketClient.GetManagementChannel();
        await _managementChannel.Log("Nibiru Connector online");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordSocketClient.StopAsync();
        if (_managementChannel is not null)
        {
            await _managementChannel.Log("Nibiru Connector offline");
        }
    }
}
