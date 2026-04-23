using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Toarnbeike.Testing.Logging;

public sealed class TestLogger<T>(string category, ConcurrentQueue<TestLogEntry>? entries = null) : ILogger<T>
{
    private readonly ConcurrentQueue<TestLogEntry> _entries = entries ?? new ConcurrentQueue<TestLogEntry>();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        string message;

        try
        {
            message = formatter(state, exception);
        }
        catch
        {
            message = SafeToString(state);
        }

        IReadOnlyList<KeyValuePair<string, object?>>? props = null;

        if (state is IEnumerable<KeyValuePair<string, object?>> pairs)
        {
            props = pairs.ToList();
        }

        _entries.Enqueue(new TestLogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            Message = message,
            Exception = exception,
            Category = category,
            Timestamp = DateTime.UtcNow,
            Properties = props
        });
    }

    public LogAssertionScope Assert() => new(_entries.ToArray());

    private static string SafeToString(object? state)
    {
        switch (state)
        {
            case null:
                return "<null>";
            // Structured logging pad
            case IEnumerable<KeyValuePair<string, object?>> kvps:
                try
                {
                    var dict = kvps.ToDictionary(k => k.Key, v => v.Value);

                    // Originele message template zit meestal onder {OriginalFormat}
                    if (dict.TryGetValue("{OriginalFormat}", out var template))
                    {
                        return template?.ToString() ?? "<no template>";
                    }

                    // Fallback: key=value lijst
                    return string.Join(", ", dict.Select(kv => $"{kv.Key}={kv.Value}"));
                }
                catch
                {
                    // return nothing in the switch, zodat we de fallback naar state.ToString() gebruiken.
                }

                break;
        }

        try
        {
            return state.ToString() ?? "<null>";
        }
        catch
        {
            return $"<{state.GetType().Name}>";
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}