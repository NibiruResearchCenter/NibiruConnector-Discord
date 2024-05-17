using Serilog.Events;

namespace NibiruConnector.Options;

public record LoggerOptions
{
    public LogEventLevel Level { get; set; } = LogEventLevel.Information;

    public string FilePath { get; set; } = string.Empty;
}
