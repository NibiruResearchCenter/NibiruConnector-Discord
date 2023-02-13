// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.ComponentModel;
using NibiruConnector.Attributes;
using NibiruConnector.Commands.AutoComplete;
using NibiruConnector.Extensions;
using NibiruConnector.Interfaces;
using NibiruConnector.Models;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace NibiruConnector.Commands;

[RegisterCommand]
public sealed class DataSyncCommand : CommandGroup
{
    private readonly FeedbackService _feedbackService;
    private readonly IRconService _rconService;

    public DataSyncCommand(FeedbackService feedbackService, IRconService rconService)
    {
        _feedbackService = feedbackService;
        _rconService = rconService;
    }
    
    [Command("sync-groups")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Manually sync groups.")]
    public async Task<IResult> SyncGroups()
    {
        var response = await _rconService
            .ExecuteServerCommand<GetGroupResponse>("nibiruc fetch groups");
        
        if (response.Result is not null)
        {
            GroupsAutoCompleteProvider.UpdateGroups(response.Result.Data);
        }
        
        var embed = response.BuildGetGroupEmbed("Groups");
        return await _feedbackService.SendContextualEmbedAsync(embed);
    }
    
    [Command("sync-whitelisted")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Manually sync whitelisted players. A light version of whitelist-list")]
    public async Task<IResult> SyncWhitelisted()
    {
        var response = await _rconService
            .ExecuteServerCommand<GetGroupResponse>("nibiruc fetch whitelisted");
        
        if (response.Result is not null)
        {
            WhitelistedPlayerAutoCompleteProvider.UpdateWhitelistedPlayers(response.Result.Data);
        }
        
        var embed = response.BuildGetGroupEmbed("Whitelisted Players");
        return await _feedbackService.SendContextualEmbedAsync(embed);
    }
}
