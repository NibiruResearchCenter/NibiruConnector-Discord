using MassTransit;
using Microsoft.AspNetCore.Mvc;
using NibiruConnector.Contracts;
using NibiruConnector.Models;

namespace NibiruConnector.Controllers;

[ApiController]
[Route("/webhook")]
public class WebhookController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public WebhookController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("uptime-kuma")]
    public async Task<IActionResult> ReceiveUptimeKumaWebhookAsync(
        [FromBody] UptimeKumaWebhook body,
        [FromQuery] string channels)
    {
        if (string.IsNullOrEmpty(channels))
        {
            return BadRequest();
        }

        var refs = channels.Split(",");
        if (refs.Length == 0)
        {
            return BadRequest();
        }

        var contract = new UptimeKumaStatusUpdate
        {
            Data = body,
            ChannelRefs = refs
        };

        await _publishEndpoint.Publish(contract);

        return NoContent();
    }
}
