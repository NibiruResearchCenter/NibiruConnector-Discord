// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NibiruConnector.Commands.AutoComplete;
using NibiruConnector.Extensions;
using NibiruConnector.Interfaces;
using NibiruConnector.Models;

namespace NibiruConnector.Jobs;

public class DataSyncJobs : BackgroundService
{
    private readonly IRconService _rconService;
    private readonly ILogger<DataSyncJobs> _logger;
    private readonly PeriodicTimer _timer;
    
    public DataSyncJobs(IRconService rconService, ILogger<DataSyncJobs> logger)
    {
        _rconService = rconService;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromHours(1));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            await UpdateGroups();
            await UpdateWhitelistedPlayers();
        } while (await _timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task UpdateGroups()
    {
        try
        {
            var response = await _rconService
                .ExecuteServerCommand<GetGroupResponse>("nibiruc fetch groups");

            if (response.Result is not null)
            {
                GroupsAutoCompleteProvider.UpdateGroups(response.Result.Data);
            }
            else
            {
                _logger.LogWarning("Failed to sync groups data.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Caught an Exception {ExceptionName} when sync groups data.", e.GetBaseException().GetType().FullName ?? "Unknown");
        }
    }
    
    private async Task UpdateWhitelistedPlayers()
    {
        try
        {
            var response = await _rconService
                .ExecuteServerCommand<GetGroupResponse>("nibiruc fetch whitelisted");

            if (response.Result is not null)
            {
                WhitelistedPlayerAutoCompleteProvider.UpdateWhitelistedPlayers(response.Result.Data);
            }
            else
            {
                _logger.LogWarning("Failed to sync whitelisted player data.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Caught an Exception {ExceptionName} when sync whitelisted player data.", e.GetBaseException().GetType().FullName ?? "Unknown");
        }
    }
}
