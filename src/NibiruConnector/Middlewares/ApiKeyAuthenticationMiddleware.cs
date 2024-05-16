namespace NibiruConnector.Middlewares;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (string.IsNullOrEmpty(Configuration.ApiKey))
        {
            await _next(context);
            return;
        }

        context.Request.Headers.TryGetValue("ApiKey", out var apiKey);
        if (apiKey.Count != 1)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (apiKey[0] != Configuration.ApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
