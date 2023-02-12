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
            var response = await _rconService
                .ExecuteServerCommand<GetGroupResponse>("nibiruc fetch groups");

            if (response.Result is not null)
            {
                GroupsAutoCompleteProvider.UpdateGroups(response.Result.Groups.ToArray());
            }
            else
            {
                _logger.LogWarning("Failed to sync groups data.");
            }
        } while (await _timer.WaitForNextTickAsync(stoppingToken));
    }
}
