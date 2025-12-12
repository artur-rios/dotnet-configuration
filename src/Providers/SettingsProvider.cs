using ArturRios.Extensions;
using Microsoft.Extensions.Configuration;

namespace ArturRios.Configuration.Providers;

using IConfigurationProvider = Interfaces.IConfigurationProvider;

/// <summary>
/// Provides configuration values from an <see cref="IConfiguration"/> source (e.g., appsettings.json, environment variables bound to configuration).
/// </summary>
/// <remarks>
/// Values are retrieved as strings and parsed using helper extension methods into booleans, integers, or deserialized objects.
/// </remarks>
public class SettingsProvider(IConfiguration configuration) : IConfigurationProvider
{
    /// <summary>
    /// Gets a boolean value for the given configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The parsed boolean value, or <c>null</c> if not found or unparseable.</returns>
    public bool? GetBool(string key)
    {
        var value = configuration[key];

        return value.ParseToBoolOrDefault();
    }

    /// <summary>
    /// Gets an integer value for the given configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The parsed integer value, or <c>null</c> if not found or unparseable.</returns>
    public int? GetInt(string key)
    {
        var value = configuration[key];

        return value.ParseToIntOrDefault();
    }

    /// <summary>
    /// Gets the raw string value for the given configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The string value, or <c>null</c> if not found.</returns>
    public string? GetString(string key) => configuration[key];

    /// <summary>
    /// Gets a deserialized object of type <typeparamref name="T"/> for the given configuration key.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The deserialized object instance, or <c>null</c> if not found or unparseable.</returns>
    public T? GetObject<T>(string key) where T : class
    {
        var value = configuration[key];

        return value.ParseToObjectOrDefault<T>();
    }
}
