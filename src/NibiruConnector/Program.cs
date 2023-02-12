// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NibiruConnector;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateDefaultBuilder();

builder.AddDiscordService(services =>
{
    // Inject Options
    return "";
});

builder.ConfigureServices((_, services) =>
{
    services.AddOptions();
    services.AddNibiruServices();
    services.AddDiscordCommands(enableSlash: true);
    services.AddDiscordCommandTrees();
});

builder.ConfigureLogging((_, loggingBuilder) =>
{
    loggingBuilder.ClearProviders();
});

builder.UseSerilog();

var app = builder.Build();

var slashService = app.Services.GetRequiredService<SlashService>();
var updateSlash = await slashService.UpdateSlashCommandsAsync(new Snowflake(1074046319469011084));
if (!updateSlash.IsSuccess)
{
    Log.Warning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
}

app.Run();
