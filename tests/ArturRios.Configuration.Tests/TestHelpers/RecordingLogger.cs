using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ArturRios.Configuration.Tests.TestHelpers;

/// <summary>
///     Keeps the level and formatted message of every entry logged to it.
/// </summary>
public sealed class RecordingLogger<T> : ILogger<T>
{
    public ConcurrentQueue<(LogLevel Level, string Message)> Entries { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        Entries.Enqueue((logLevel, formatter(state, exception)));
}
