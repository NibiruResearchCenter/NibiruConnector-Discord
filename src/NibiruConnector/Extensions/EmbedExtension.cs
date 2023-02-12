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
                let playerString = string.Join("; ", pg.Players)
                select new EmbedField($"Group: {pg.GroupName}", playerString)
            ).ToArray();

        return cer.BuildSuccessCommandEmbed(fields);
    }
    
    public static Embed BuildGetGroupEmbed(this CommandExecutionResult<GetGroupResponse> cer)
    {
        var failed = cer.Result is null;
        if (failed)
        {
            return cer.BuildFailedCommandEmbed();
        }

        var groups = string.Join("; ", cer.Result!.Groups);
        return cer.BuildSuccessCommandEmbed(new EmbedField("Groups", groups));
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
                new("Time Consumption", cer.TimeConsumption.ToString()),
                new("Message", cer.Response)
            }
        };
    }

    private static Embed BuildSuccessCommandEmbed<T>(this CommandExecutionResult<T> cer, params EmbedField[] extraFields)
    {
        var fields = new List<EmbedField>
        {
            new("Result", "Success"),
            new("Time Consumption", cer.TimeConsumption.ToString())
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
