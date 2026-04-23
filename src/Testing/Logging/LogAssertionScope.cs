using Microsoft.Extensions.Logging;

namespace Toarnbeike.Testing.Logging;

public sealed class LogAssertionScope(IReadOnlyList<TestLogEntry> entries)
{
    public LogAssertionScope WithLevel(LogLevel level)
    {
        return new LogAssertionScope(
            entries.Where(e => e.LogLevel == level).ToList());
    }

    public LogAssertionScope WithMessageContaining(string text)
    {
        return new LogAssertionScope(
            entries.Where(e =>
                e.Message.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList());
    }

    public LogAssertionScope WithException<TException>()
        where TException : Exception
    {
        return new LogAssertionScope(
            entries.Where(e => e.Exception is TException).ToList());
    }

    public LogAssertionScope WithProperty(string key, object? value)
    {
        return new LogAssertionScope(
            entries.Where(e =>
                    e.Properties != null &&
                    e.Properties.Any(p =>
                        p.Key == key && Equals(p.Value, value)))
                .ToList());
    }

    public LogAssertionScope WithCategory(string category)
    {
        return new LogAssertionScope(
            entries.Where(e =>
                string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase)).ToList());
    }

    // Terminal operations

    public TestLogEntry Single()
    {
        return entries.Count == 1 
            ? entries[0] 
            : throw new LogAssertionException($"Expected exactly one log entry, but found {entries.Count}\n{Dump(entries)}", entries);
    }

    public void None()
    {
        if (entries.Count != 0)
        {
            throw new LogAssertionException($"Expected no log entries, but found {entries.Count}\n{Dump(entries)}", entries);
        }
    }

    public void AtLeastOnce()
    {
        if (entries.Count == 0)
        {
            throw new LogAssertionException("Expected at least one log entry, but found none", entries);
        }
    }

    public IReadOnlyList<TestLogEntry> All() => entries;

    private static string Dump(IEnumerable<TestLogEntry> entries)
    {
        return string.Join(Environment.NewLine,
            entries.Select(e => $"[{e.LogLevel}] {e.Message}"));
    }
}