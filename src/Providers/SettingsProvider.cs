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
/// <param name="configuration">The configuration to read from.</param>
/// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <c>null</c>.</exception>
public class SettingsProvider(IConfiguration configuration) : IConfigurationProvider
{
    private readonly IConfiguration _configuration =
        configuration ?? throw new ArgumentNullException(nameof(configuration));

    /// <summary>
    /// Gets a boolean value for the given configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The parsed boolean value, or <c>null</c> if not found or unparseable.</returns>
    public bool? GetBool(string key)
    {
        var value = _configuration[key];

        return value.ParseToBoolOrDefault();
    }

    /// <summary>
    /// Gets an integer value for the given configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The parsed integer value, or <c>null</c> if not found or unparseable.</returns>
    public int? GetInt(string key)
    {
        var value = _configuration[key];

        return value.ParseToIntOrDefault();
    }

    /// <summary>
    /// Gets the raw string value for the given configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The string value, or <c>null</c> if not found.</returns>
    public string? GetString(string key) => _configuration[key];

    /// <summary>
    /// Gets a deserialized object of type <typeparamref name="T"/> for the given configuration key.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The deserialized object instance, or <c>null</c> if not found or unparseable.</returns>
    /// <remarks>
    /// The value stored under <paramref name="key"/> must itself be a JSON string. A section such as
    /// <c>"Database": { "Host": "..." }</c> in an appsettings file has no value of its own — its children do —
    /// so it reads as <c>null</c>; bind a section with <c>configuration.GetSection(key).Get&lt;T&gt;()</c> from
    /// Microsoft.Extensions.Configuration.Binder instead.
    /// </remarks>
    public T? GetObject<T>(string key) where T : class
    {
        var value = _configuration[key];

        return value.ParseToObjectOrDefault<T>();
    }
}
