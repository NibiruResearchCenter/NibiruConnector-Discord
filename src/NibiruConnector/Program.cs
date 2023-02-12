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
    services.AddNibiruJobs();
    services.AddNibiruOptions();
    services.AddNibiruServices();
    services.AddDiscordCommandTrees();
    services.AddDiscordAutoCompleteProviders();
});

builder.ConfigureLogging((_, loggingBuilder) =>
{
    loggingBuilder.ClearProviders();
});

builder.UseSerilog();

var app = builder.Build();

var _ = app.Services.GetRequiredService<IRconService>();

var slashService = app.Services.GetRequiredService<SlashService>();
var discordOptions = app.Services.GetRequiredService<IOptions<DiscordOptions>>();
var updateSlash = await slashService.UpdateSlashCommandsAsync(new Snowflake(discordOptions.Value.GuildId));
if (!updateSlash.IsSuccess)
{
    Log.Warning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
}

app.Run();
