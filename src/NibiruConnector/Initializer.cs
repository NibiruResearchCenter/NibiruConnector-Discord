// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NibiruConnector.Attributes;
using NibiruConnector.Interfaces;
using NibiruConnector.Services;
using Remora.Commands.Extensions;

namespace NibiruConnector;

public static class Initializer
{
    public static IServiceCollection AddDiscordCommandTrees(this IServiceCollection services)
    {
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

    public static IServiceCollection AddNibiruServices(this IServiceCollection services)
    {
        services.AddSingleton<IRconService, RconService>();
        
        return services;
    }
}
