using NibiruConnector.Options.Api;

namespace NibiruConnector.Options;

public record ApiOptions
{
    public List<ApiTokenOptions> Tokens { get; set; } = [];
}
