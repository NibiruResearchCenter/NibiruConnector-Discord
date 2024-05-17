using Microsoft.Extensions.Options;
using NibiruConnector.Options;

namespace NibiruConnector.Middlewares;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    private readonly Dictionary<string, string> _apiKeys;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyAuthenticationMiddleware> logger,
        IOptions<ApiOptions> options)
    {
        _next = next;
        _logger = logger;
        _apiKeys = options.Value.Tokens
            .ToDictionary(x => x.Token, x => x.Ref);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_apiKeys.Count == 0)
        {
            await _next(context);
            return;
        }

        context.Request.Headers.TryGetValue("ApiKey", out var apiKey);
        if (apiKey.Count != 1)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _logger.LogWarning("No ApiKey header found in request");
            return;
        }

        var key = apiKey[0]!;

        if (_apiKeys.TryGetValue(key, out var keyRef) is false)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _logger.LogWarning("Somebody tried to use invalid ApiKey {Key}", key);
            return;
        }

        _logger.LogInformation("Received request using key {Ref}", keyRef);

        await _next(context);
    }
}
