// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Diagnostics;
using System.Text.Json;
using NibiruConnector.Interfaces;
using NibiruConnector.Models;

namespace NibiruConnector.Extensions;

public static class CommandExtension
{
    public static async Task<CommandExecutionResult<T>> ExecuteServerCommand<T>(this IRconService rconService, string command)
    {
        var sw = Stopwatch.StartNew();
        var response = await rconService.SendCommand(command);
        sw.Stop();

        var cer = new CommandExecutionResult<T>(command, response, sw.ElapsedMilliseconds);
        
        try
        {
            var parsed = JsonSerializer.Deserialize<T>(response);
            cer.Result = parsed;
        }
        catch (Exception)
        {
            // ignored
        }

        return cer;
    }
}
