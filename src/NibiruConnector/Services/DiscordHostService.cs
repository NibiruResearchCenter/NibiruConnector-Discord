using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NibiruConnector.Extensions;
using NibiruConnector.Options;

namespace NibiruConnector.Services;

public class DiscordHostService : IHostedService
{
    private readonly IOptions<DiscordOptions> _options;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<DiscordHostService> _logger;
    private IMessageChannel? _managementChannel;

    public DiscordHostService(
        IOptions<DiscordOptions> options,
        DiscordSocketClient discordSocketClient,
        ILogger<DiscordHostService> logger)
    {
        _options = options;
        _discordSocketClient = discordSocketClient;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordSocketClient.LoginAsync(TokenType.Bot, _options.Value.BotToken);

        _discordSocketClient.Log += msg =>
        {
            _logger.Log(msg.Severity.MapToLogLevel(), msg.Exception,
                "{Source} - {Message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        };

        await _discordSocketClient.StartAsync();

        _managementChannel = await _discordSocketClient.GetMessageChannel(_options.Value.ManagementChannel);
        await _managementChannel.Log("Nibiru Connector online");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordSocketClient.StopAsync();
        if (_managementChannel is not null)
        {
            await _managementChannel.Log("Nibiru Connector offline");
        }
    }
}
