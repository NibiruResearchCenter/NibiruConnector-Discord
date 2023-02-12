// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using NibiruConnector.Attributes;
using NibiruConnector.Interfaces;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;

namespace NibiruConnector.Commands;

[RegisterCommand]
public sealed class RconExecuteCommand : CommandGroup
{
    private readonly FeedbackService _feedbackService;
    private readonly IRconService _rconService;

    public RconExecuteCommand(FeedbackService feedbackService, IRconService rconService)
    {
        _feedbackService = feedbackService;
        _rconService = rconService;
    }
    
    [Command("execute")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Execute a command on the server through RCON.")]
    public async Task<IResult> ExecuteAsync(
        [Description("The command that will be executed on the server.")] string command,
        [Description("Keep Minecraft formatting code. The default is False.")] bool keepMinecraftFormatting = false)
    {
        var color = Color.Green;
        var fields = new List<EmbedField>();
        
        try
        {
            var sw = Stopwatch.StartNew();
            var response = await _rconService.SendCommand(command, keepMinecraftFormatting);
            sw.Stop();
            fields.Add(new EmbedField("Response", response));
            fields.Add(new EmbedField("Time Consumption", $"{sw.ElapsedMilliseconds} ms"));
        }
        catch (Exception e)
        {
            color = Color.Red;
            fields.Add(new EmbedField("Exception Type", e.GetBaseException().GetType().FullName ?? "Unknown"));
            fields.Add(new EmbedField("Exception Message", e.Message));
            fields.Add(new EmbedField("Stack Trace", e.StackTrace ?? "Unknown"));
        }
        
        var embed = new Embed
        {
            Title = "RCON Execute Result",
            Description = $"Command: {command}",
            Colour = color,
            Fields = fields
        };
        
        return await _feedbackService.SendContextualEmbedAsync(embed);
    }
}
