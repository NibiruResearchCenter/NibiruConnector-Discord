using NibiruConnector;

var builder = WebApplication.CreateBuilder();

builder.Configuration.AddConfiguration();

builder.Services.AddControllers();

builder.Services.AddLogger();
builder.Services.AddDiscord();
builder.Services.AddConnectorOptions();
builder.Services.AddConnectorTransit();

var app = builder.Build();

app.UseApiKeyAuthentication();

app.MapControllers();

await app.RunAsync();
