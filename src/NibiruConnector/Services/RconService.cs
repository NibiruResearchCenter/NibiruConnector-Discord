// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Net;
using CoreRCON;
using CoreRCON.PacketFormats;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NibiruConnector.Exceptions;
using NibiruConnector.Interfaces;
using NibiruConnector.Options;

namespace NibiruConnector.Services;

public class RconService : IRconService
{
    private readonly ILogger<RconService> _logger;

    private enum RconStatus
    {
        Disconnected,
        Connected,
    }
    
    private readonly RCON _rcon;
    private RconStatus _rconStatus;

    public RconService(ILogger<RconService> logger, IOptions<RconOptions> options)
    {
        _logger = logger;

        _rconStatus = RconStatus.Disconnected;
        _rcon = new RCON(
            IPAddress.Parse(options.Value.IpAddress),
            (ushort)options.Value.Port,
            options.Value.Password);
        _rcon.OnPacketReceived += OnRconPacketReceived;
        _rcon.OnDisconnected += OnRconDisconnected;

        Connect().ConfigureAwait(false).GetAwaiter().GetResult();
    }
    
    public async Task<string> SendCommand(string command)
    {
        var checkTimes = 0;
        while (_rconStatus == RconStatus.Disconnected && checkTimes <= 3)
        {
            checkTimes++;
            _logger.LogInformation("RCON is disconnected, waiting for reconnect... ({CheckTimes}/3)", checkTimes);
            await Task.Delay(1000);

            if (_rconStatus == RconStatus.Disconnected && checkTimes == 3)
            {
                throw new RconException("RCON is disconnected.");
            }
        }
        
        return await _rcon.SendCommandAsync(command);
    }
    
    private async Task Connect()
    {
        _logger.LogInformation("RCON connecting...");
        await _rcon.ConnectAsync();
        _rconStatus = RconStatus.Connected;
        _logger.LogInformation("RCON connected.");
    }
    
    private void OnRconPacketReceived(RCONPacket packet)
    {
        _logger.LogDebug("[RCON SERVER] [S{RconPacketId:00000}] [{RconPacketType}] {RconPacketBody}",
            packet.Id, packet.Type.ToString(), packet.Body);
    }

    private void OnRconDisconnected()
    {
        _logger.LogInformation("RCON disconnected.");
        _rconStatus = RconStatus.Disconnected;
    }
}
