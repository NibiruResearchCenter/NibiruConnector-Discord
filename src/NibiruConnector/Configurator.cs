using System.Globalization;
using System.Net;
using Discord;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using MassTransit;
using NibiruConnector.Middlewares;
using NibiruConnector.Options;
using NibiruConnector.Services;
using Serilog;
using Serilog.Events;

namespace NibiruConnector;

public static class Configurator
{
    public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder builder)
    {
        var cfg = new ConfigurationBuilder();

        cfg.AddYamlFile(Configuration.ConfigurationFilePath, false);

        if (Configuration.RuntimeEnvironment is not "production")
        {
            cfg.AddYamlFile($"appsettings.{Configuration.RuntimeEnvironment}.yaml", true);
        }

        Configuration.Instance = cfg.Build();

        builder.AddConfiguration(Configuration.Instance);

        return builder;
    }

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
                GatewayIntents = GatewayIntents.None,
                WebSocketProvider = DefaultWebSocketProvider.Create(webProxy),
                RestClientProvider = DefaultRestClientProvider.Create(useProxy),
            };
        });
        services.AddSingleton<DiscordSocketClient>();
        services.AddHostedService<DiscordHostService>();
        services.AddSingleton<DiscordLogger>();

        return services;
    }

    public static IServiceCollection AddConnectorTransit(this IServiceCollection services)
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

    public static IServiceCollection AddConnectorOptions(this IServiceCollection services)
    {
        services.AddOptions<LoggerOptions>().BindConfiguration("logger");
        services.AddOptions<DiscordOptions>().BindConfiguration("discord");
        services.AddOptions<ApiOptions>().BindConfiguration("api");

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
        var loggerOptions = Configuration.Instance.GetValue<LoggerOptions>("logger") ?? new LoggerOptions();

        var loggerConfiguration = new LoggerConfiguration();

        loggerConfiguration.WriteTo.Console(
            outputTemplate: LoggerOutputTemplate,
            formatProvider: CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(loggerOptions.FilePath) is false)
        {
            loggerConfiguration.WriteTo.File(
                outputTemplate: LoggerOutputTemplate,
                path: loggerOptions.FilePath,
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.InvariantCulture);
        }

        loggerConfiguration.MinimumLevel.Is(loggerOptions.Level);

        loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        loggerConfiguration.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

        Log.Logger = loggerConfiguration.CreateLogger();
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(Log.Logger);
    }
}
