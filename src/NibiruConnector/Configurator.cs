using System.Globalization;
using System.Net;
using Discord;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using MassTransit;
using NibiruConnector.Extensions;
using NibiruConnector.Middlewares;
using NibiruConnector.Services;
using Serilog;
using Serilog.Events;

namespace NibiruConnector;

public static class Configurator
{
    public static IServiceCollection AddLogger(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.ConfigureLogger();
        });

        return services;
    }

    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        services.AddSingleton<DiscordSocketConfig>(_ =>
        {
            var webProxy = string.IsNullOrEmpty(Configuration.HttpsProxy)
                ? null
                : new WebProxy(Configuration.HttpsProxy);
            var useProxy = webProxy is not null;

            return new DiscordSocketConfig
            {
                MessageCacheSize = 100,
                WebSocketProvider = DefaultWebSocketProvider.Create(webProxy),
                RestClientProvider = DefaultRestClientProvider.Create(useProxy),
            };
        });
        services.AddSingleton<DiscordSocketClient>();
        services.AddHostedService<DiscordHostService>();

        services.AddKeyedSingleton<IMessageChannel>("Notification", (sp, _) =>
        {
            var discordSocketClient = sp.GetRequiredService<DiscordSocketClient>();
            return discordSocketClient.GetNotificationChannel().GetAwaiter().GetResult();
        });
        services.AddKeyedSingleton<IMessageChannel>("Management", (sp, _) =>
        {
            var discordSocketClient = sp.GetRequiredService<DiscordSocketClient>();
            return discordSocketClient.GetManagementChannel().GetAwaiter().GetResult();
        });

        services.AddSingleton<DiscordLogger>();

        return services;
    }

    public static IServiceCollection AddLocalTransit(this IServiceCollection services)
    {
        services.AddMassTransit(c =>
        {
            c.AddConsumers(typeof(Configurator).Assembly);
            c.UsingInMemory((ctx, cfg) =>
            {
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }

    public static WebApplication UseApiKeyAuthentication(this WebApplication app)
    {
        app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

        return app;
    }

    private const string LoggerOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    private static void ConfigureLogger(this ILoggingBuilder loggingBuilder)
    {
        var loggerConfiguration = new LoggerConfiguration();

        loggerConfiguration.WriteTo.Console(
            outputTemplate: LoggerOutputTemplate,
            formatProvider: CultureInfo.InvariantCulture);

        var fileOutput = Configuration.LoggerFileOutput;
        if (string.IsNullOrEmpty(fileOutput) is false)
        {
            loggerConfiguration.WriteTo.File(
                outputTemplate: LoggerOutputTemplate,
                path: fileOutput,
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture);
        }

        var level = Enum.Parse<LogEventLevel>(Configuration.LoggerLevel);
        loggerConfiguration.MinimumLevel.Is(level);

        Log.Logger = loggerConfiguration.CreateLogger();
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(Log.Logger);
    }
}
