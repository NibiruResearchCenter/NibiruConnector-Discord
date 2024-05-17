namespace NibiruConnector.Options.Api;

public record ApiTokenOptions
{
    public string Token { get; set; } = string.Empty;

    public string Ref { get; set; } = string.Empty;
}
