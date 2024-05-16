using System.Text.Json;
using System.Text.Json.Serialization;

namespace NibiruConnector.Models;

public record UptimeKumaWebhook
{
    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("monitor")]
    public JsonElement? Monitor { get; set; } = null;

    [JsonPropertyName("heartbeat")]
    public JsonElement? Heartbeat { get; set; } = null;
}
