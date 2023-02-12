// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NibiruConnector.Attributes;
using NibiruConnector.Commands.AutoComplete;
using NibiruConnector.Interfaces;
using NibiruConnector.Jobs;
using NibiruConnector.Options;
using NibiruConnector.Services;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Serilog;

namespace NibiruConnector;

public static class Initializer
{
    public static void InitializeLogger()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development")
        {
            loggerConfiguration.MinimumLevel.Verbose();
        }
        else
        {
            loggerConfiguration.MinimumLevel.Information();
        }

        Log.Logger = loggerConfiguration.CreateLogger();
    }
    
    public static void InitializeConfiguration(this IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
        {
            new("Discord:Token", Environment.GetEnvironmentVariable("DISCORD_TOKEN")),
            new("Discord:GuildId", Environment.GetEnvironmentVariable("DISCORD_GUILD_ID")),
            new("Rcon:IpAddress", Environment.GetEnvironmentVariable("RCON_IP_ADDRESS")),
            new("Rcon:Port", Environment.GetEnvironmentVariable("RCON_PORT")),
            new("Rcon:Password", Environment.GetEnvironmentVariable("RCON_PASSWORD")),
        });
    }
    
    public static IServiceCollection AddDiscordCommandTrees(this IServiceCollection services)
    {
        services.AddDiscordCommands(enableSlash: true);
        
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsSealed: true })
            .Where(x => x.GetCustomAttributes<RegisterCommandAttribute>().Any())
            .ToArray();
        
        var builder = services.AddCommandTree();
        
        foreach (var type in types)
        {
            builder.WithCommandGroup(type);
        }

        return builder.Finish();
    }

    public static IServiceCollection AddDiscordAutoCompleteProviders(this IServiceCollection services)
    {
        services.AddAutocompleteProvider<GroupsAutoCompleteProvider>();
        
        return services;
    }

    public static IServiceCollection AddNibiruServices(this IServiceCollection services)
    {
        services.AddSingleton<IRconService, RconService>();
        
        return services;
    }

    public static IServiceCollection AddNibiruJobs(this IServiceCollection services)
    {
        services.AddHostedService<DataSyncJobs>();
        
        return services;
    }
    
    public static IServiceCollection AddNibiruOptions(this IServiceCollection services)
    {
        services.AddOptions<DiscordOptions>().BindConfiguration("Discord");
        services.AddOptions<RconOptions>().BindConfiguration("Rcon");

        return services;
    }
}
