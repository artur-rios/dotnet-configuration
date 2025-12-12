namespace ArturRios.Configuration.Providers.Interfaces;

/// <summary>
/// Abstraction for retrieving configuration values from different sources.
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Retrieves a boolean value for the specified key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The parsed boolean value, or <c>null</c> if not found or unparseable.</returns>
    bool? GetBool(string key);

    /// <summary>
    /// Retrieves an integer value for the specified key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The parsed integer value, or <c>null</c> if not found or unparseable.</returns>
    int? GetInt(string key);

    /// <summary>
    /// Retrieves a string value for the specified key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>The string value, or <c>null</c> if not found.</returns>
    string? GetString(string key);

    /// <summary>
    /// Retrieves an object of type <typeparamref name="T"/> for the specified key.
    /// </summary>
    /// <typeparam name="T">The target type to deserialize to.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The deserialized object, or <c>null</c> if not found or unparseable.</returns>
    T? GetObject<T>(string key) where T : class;
}
