namespace Toarnbeike.Testing.Logging;

public class LogAssertionException(string message, IEnumerable<TestLogEntry> entries) : Exception(message)
{
    public IReadOnlyList<TestLogEntry> Entries { get; } = entries.ToList();
}