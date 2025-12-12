using ArturRios.Configuration.Providers.Interfaces;
using ArturRios.Extensions;

namespace ArturRios.Configuration.Providers;

/// <summary>
/// Provides configuration values by reading environment variables from the current process.
/// </summary>
/// <remarks>
/// Values are retrieved via <see cref="Environment.GetEnvironmentVariable(string)"/> and parsed using helper extension methods into booleans, integers, or deserialized objects.
/// </remarks>
public class EnvironmentProvider : IConfigurationProvider
{
    /// <summary>
    /// Gets a boolean value for the given environment variable key.
    /// </summary>
    /// <param name="key">The environment variable key.</param>
    /// <returns>The parsed boolean value, or <c>null</c> if not found or unparseable.</returns>
    public bool? GetBool(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        return value.ParseToBoolOrDefault();
    }

    /// <summary>
    /// Gets an integer value for the given environment variable key.
    /// </summary>
    /// <param name="key">The environment variable key.</param>
    /// <returns>The parsed integer value, or <c>null</c> if not found or unparseable.</returns>
    public int? GetInt(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        return value.ParseToIntOrDefault();
    }

    /// <summary>
    /// Gets the raw string value for the given environment variable key.
    /// </summary>
    /// <param name="key">The environment variable key.</param>
    /// <returns>The string value, or <c>null</c> if not found.</returns>
    public string? GetString(string key) => Environment.GetEnvironmentVariable(key);

    /// <summary>
    /// Gets a deserialized object of type <typeparamref name="T"/> for the given environment variable key.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize to.</typeparam>
    /// <param name="key">The environment variable key.</param>
    /// <returns>The deserialized object instance, or <c>null</c> if not found or unparseable.</returns>
    public T? GetObject<T>(string key) where T : class
    {
        var value = Environment.GetEnvironmentVariable(key);

        return value.ParseToObjectOrDefault<T>();
    }
}
