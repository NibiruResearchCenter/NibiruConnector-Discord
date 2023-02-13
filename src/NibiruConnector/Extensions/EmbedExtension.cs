// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Drawing;
using NibiruConnector.Models;
using Remora.Discord.API.Objects;

namespace NibiruConnector.Extensions;

public static class EmbedExtension
{
    public static Embed BuildGeneralCommandEmbed(this CommandExecutionResult<GeneralCommandResponse> cer)
    {
        var failed = cer.Result is null;
        if (failed)
        {
            return cer.BuildFailedCommandEmbed();
        }

        return cer.BuildSuccessCommandEmbed(
            new EmbedField("Message", cer.Result!.Message));
    }

    public static Embed BuildWhitelistListEmbed(this CommandExecutionResult<WhitelistListResponse> cer)
    {
        var failed = cer.Result is null;
        if (failed)
        {
            return cer.BuildFailedCommandEmbed();
        }

        var fields = (
                from pg in cer.Result!.PlayerGroups
                let playerString = string.Join("; ", pg.Players
                    .OrderBy(x => x.DaysSinceLastJoin)
                    .Select(x => $"{x.PlayerName} ({x.DaysSinceLastJoin})"))
                select new EmbedField($"Group: {pg.GroupName}", playerString)
            ).ToList();
        var allPlayers = cer.Result!.PlayerGroups
            .SelectMany(x => x.Players)
            .OrderBy(x => x.DaysSinceLastJoin)
            .ToArray();

        var neverJoin = allPlayers
            .Where(x => x.DaysSinceLastJoin < 0);

        var above7Days = allPlayers
            .Where(x => x.DaysSinceLastJoin >= 7);

        var above30Days = allPlayers
            .Where(x => x.DaysSinceLastJoin >= 30);
        
        fields.Add(new EmbedField("[EXTRA] Never Join",
            string.Join("; ", neverJoin
                .Select(x => $"{x.PlayerName} ({x.DaysSinceLastJoin})"))));
        fields.Add(new EmbedField("[EXTRA] Above 7 Days",
            string.Join("; ", above7Days
                .Select(x => $"{x.PlayerName} ({x.DaysSinceLastJoin})"))));
        fields.Add(new EmbedField("[EXTRA] Above 30 Days",
            string.Join("; ", above30Days
                .Select(x => $"{x.PlayerName} ({x.DaysSinceLastJoin})"))));
        
        return cer.BuildSuccessCommandEmbed(fields.ToArray());
    }
    
    public static Embed BuildGetGroupEmbed(this CommandExecutionResult<GetGroupResponse> cer, string title)
    {
        var failed = cer.Result is null;
        if (failed)
        {
            return cer.BuildFailedCommandEmbed();
        }

        var groups = string.Join("; ", cer.Result!.Data);
        return cer.BuildSuccessCommandEmbed(new EmbedField(title, groups));
    }

    private static Embed BuildFailedCommandEmbed<T>(this CommandExecutionResult<T> cer)
    {
        return new Embed
        {
            Title = "Command result",
            Colour = Color.Red,
            Description = $"Command: {cer.Command}",
            Fields = new List<EmbedField>
            {
                new("Result", "Failed"),
                new("Time Consumption", $"{cer.TimeConsumption.ToString()} ms"),
                new("Message", cer.Response)
            }
        };
    }

    private static Embed BuildSuccessCommandEmbed<T>(this CommandExecutionResult<T> cer, params EmbedField[] extraFields)
    {
        var fields = new List<EmbedField>
        {
            new("Result", "Success"),
            new("Time Consumption", $"{cer.TimeConsumption.ToString()} ms")
        };
        fields.AddRange(extraFields);
        return new Embed
        {
            Title = "Command Result",
            Colour = Color.Green,
            Description = $"Command: {cer.Command}",
            Fields = fields
        };
    }
}
