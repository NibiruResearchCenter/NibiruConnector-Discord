using Discord;
using NibiruConnector.Exceptions;
using NibiruConnector.Models;

namespace NibiruConnector.Extensions;

public static class DiscordMessageExtension
{
    public static Embed GetStatusChangedEmbed(this UptimeKumaWebhook data)
    {
        if (data.Heartbeat is null)
        {
            return BuildStatusChangedEmbed(
                "[TEST] Status Update Message Test",
                Color.LightGrey,
                DateTimeOffset.UtcNow,
                "Test Service",
                "UTC",
                DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                data.Message);
        }

        var heartbeat = data.Heartbeat.GetValueOrDefault();
        var status = heartbeat.GetProperty("status").GetInt32();
        var tz = heartbeat.GetProperty("timezoneOffset").GetString() ?? "Unknown";
        var time = heartbeat.GetProperty("time").GetString() is null
            ? DateTimeOffset.UtcNow
            : DateTimeOffset.Parse(heartbeat.GetProperty("time").GetString()!);
        var localTime = heartbeat.GetProperty("localDateTime").GetString() ?? "Unknown";
        var msg = heartbeat.GetProperty("msg").GetString() ?? data.Message;

        var serviceName = data.Monitor?.GetProperty("name").GetString() ?? "Unknown";

        switch (status)
        {
            // DOWN
            case 0:
                return BuildStatusChangedEmbed(
                    "❌ Server went down!",
                    Color.Red,
                    time,
                    serviceName,
                    tz,
                    localTime,
                    msg);
            // UP
            case 1:
                return BuildStatusChangedEmbed(
                    "✅ Server is up!",
                    Color.Green,
                    time,
                    serviceName,
                    tz,
                    localTime,
                    msg);
            // UNKNOWN
            default:
                throw new UnknownStatusException(status);
        }
    }

    private static Embed BuildStatusChangedEmbed(string title, Color color, DateTimeOffset timestamp, string name, string tz, string localTime, string msg)
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithColor(color)
            .WithTimestamp(timestamp)
            .WithFields(
                new EmbedFieldBuilder().WithName("Name").WithValue(name).WithIsInline(true),
                new EmbedFieldBuilder().WithName($"Time ({tz})").WithValue(localTime).WithIsInline(true),
                new EmbedFieldBuilder().WithName("Message").WithValue(msg).WithIsInline(true)
            )
            .Build();
    }
}
