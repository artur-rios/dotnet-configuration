namespace ArturRios.Configuration.Enums;

/// <summary>
/// Defines common environment names used to scope configuration.
/// </summary>
public enum EnvironmentType
{
    /// <summary>
    /// Local development environment.
    /// </summary>
    Local = 0,
    /// <summary>
    /// Shared development environment.
    /// </summary>
    Development,
    /// <summary>
    /// Pre-production environment used for validation.
    /// </summary>
    Staging,
    /// <summary>
    /// Production environment.
    /// </summary>
    Production
}
