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
public sealed class NibiruWhitelistCommand : CommandGroup
{
    private readonly FeedbackService _feedbackService;
    private readonly IRconService _rconService;

    public NibiruWhitelistCommand(FeedbackService feedbackService, IRconService rconService)
    {
        _feedbackService = feedbackService;
        _rconService = rconService;
    }
    
    [Command("whitelist-add")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Add a player to the whitelist.")]
    public async Task<IResult> WhitelistAddAsync(
        [Description("Minecraft player name")] string player,
        [Description("LuckPerms group name"), AutocompleteProvider(AutoCompleteIdentities.GROUPS)] string group)
    {
        var response = await _rconService
            .ExecuteServerCommand<GeneralCommandResponse>($"nibiruc whitelist add {player} {group}");
        if (response.Result is not null)
        {
            WhitelistedPlayerAutoCompleteProvider.AddWhitelistedPlayer(player);
        }
        var embed = response.BuildGeneralCommandEmbed();
        return await _feedbackService.SendContextualEmbedAsync(embed);
    }

    [Command("whitelist-remove")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Remove a player from the whitelist.")]
    public async Task<IResult> WhitelistRemoveAsync(
        [Description("Minecraft player name"), AutocompleteProvider(AutoCompleteIdentities.WHITELISTED_PLAYERS)] string player)
    {
        var response = await _rconService
            .ExecuteServerCommand<GeneralCommandResponse>($"nibiruc whitelist remove {player}");
        if (response.Result is not null)
        {
            WhitelistedPlayerAutoCompleteProvider.RemoveWhitelistedPlayer(player);
        }
        var embed = response.BuildGeneralCommandEmbed();
        return await _feedbackService.SendContextualEmbedAsync(embed);
    }
    
    [Command("whitelist-list")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("List all players in the whitelist.")]
    public async Task<IResult> WhitelistListAsync()
    {
        var response = await _rconService
            .ExecuteServerCommand<WhitelistListResponse>("nibiruc whitelist list");
        if (response.Result is not null)
        {
            WhitelistedPlayerAutoCompleteProvider.UpdateWhitelistedPlayers(
                response.Result.PlayerGroups
                    .SelectMany(x => x.Players)
                    .Select(x => x.PlayerName)
                    .ToList());
        }
        var embed = response.BuildWhitelistListEmbed();
        return await _feedbackService.SendContextualEmbedAsync(embed);
    }
}
