namespace ArturRios.Configuration.Tests.TestHelpers;

/// <summary>
///     Provides a disposable scope for setting and restoring environment variables during tests.
/// </summary>
public sealed class EnvVarScope : IDisposable
{
    private readonly Dictionary<string, string?> _originals = new();

    public void Dispose()
    {
        foreach (var kvp in _originals)
        {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
        }

        _originals.Clear();
    }

    public void Set(string key, string? value)
    {
        if (!_originals.ContainsKey(key))
        {
            _originals[key] = Environment.GetEnvironmentVariable(key);
        }

        Environment.SetEnvironmentVariable(key, value);
    }
}
