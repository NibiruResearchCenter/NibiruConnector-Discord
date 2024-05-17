using NibiruConnector.Models;

namespace NibiruConnector.Contracts;

public record UptimeKumaStatusUpdate
{
    public UptimeKumaWebhook Data { get; set; } = null!;

    public string[] ChannelRefs { get; set; } = [];
}
