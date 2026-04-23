using Microsoft.Extensions.Logging;

namespace Toarnbeike.Testing.Logging;

public sealed class TestLogEntry
{
    public LogLevel LogLevel { get; init; }
    public EventId EventId { get; init; }
    public string Message { get; init; } = string.Empty;
    public Exception? Exception { get; init; }
    public string Category { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public IReadOnlyList<KeyValuePair<string, object?>>? Properties { get; init; }
}