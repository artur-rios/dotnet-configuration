namespace ArturRios.Configuration.Enums;

/// <summary>
/// Describes the output classification when obtaining configuration values.
/// </summary>
public enum OutputType
{
    /// <summary>
    /// No value available.
    /// </summary>
    Void = 0,
    /// <summary>
    /// A default value is used.
    /// </summary>
    Default,
    /// <summary>
    /// A primitive value (e.g., string, number, boolean).
    /// </summary>
    Primitive,
    /// <summary>
    /// A complex object value.
    /// </summary>
    Object,
    /// <summary>
    /// An exception indicating failure.
    /// </summary>
    Exception
}
