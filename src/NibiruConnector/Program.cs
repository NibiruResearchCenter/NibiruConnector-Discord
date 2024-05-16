using NibiruConnector;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();

builder.Services.AddLogger();
builder.Services.AddDiscord();
builder.Services.AddLocalTransit();

var app = builder.Build();

app.MapControllers();

await app.RunAsync();
