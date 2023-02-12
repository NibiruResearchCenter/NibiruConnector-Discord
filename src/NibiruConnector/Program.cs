// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NibiruConnector;
using NibiruConnector.Interfaces;
using NibiruConnector.Options;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using Remora.Rest.Core;
using Serilog;

Initializer.InitializeLogger();

var builder = Host.CreateDefaultBuilder();

builder.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder.InitializeConfiguration();
});

builder.AddDiscordService(services =>
{
    var discordOptions = services.GetRequiredService<IOptions<DiscordOptions>>();
    return discordOptions.Value.Token;
});

builder.ConfigureServices((_, services) =>
{
    services.AddOptions();
    services.AddNibiruOptions();
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
var _ = app.Services.GetRequiredService<IRconService>();
var updateSlash = await slashService.UpdateSlashCommandsAsync(new Snowflake(1074046319469011084));
if (!updateSlash.IsSuccess)
{
    Log.Warning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
}

app.Run();
