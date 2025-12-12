namespace ArturRios.Configuration.Enums;

/// <summary>
/// Represents the source from which configuration values can be loaded.
/// </summary>
public enum ConfigurationSourceType
{
    /// <summary>
    /// Configuration loaded from <c>.env</c> files.
    /// </summary>
    EnvFile = 0,
    /// <summary>
    /// Configuration loaded from <c>appsettings.json</c> files.
    /// </summary>
    AppSettings,
    /// <summary>
    /// Configuration loaded directly from environment variables.
    /// </summary>
    EnvironmentVariables
}
