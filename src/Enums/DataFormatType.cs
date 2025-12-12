namespace ArturRios.Configuration.Enums;

/// <summary>
/// Specifies how configuration values or payloads are formatted.
/// </summary>
public enum DataFormatType
{
    /// <summary>
    /// JavaScript Object Notation.
    /// </summary>
    Json,
    /// <summary>
    /// Protocol Buffers binary format.
    /// </summary>
    ProtoBuf,
    /// <summary>
    /// XML format.
    /// </summary>
    Xml,
    /// <summary>
    /// Plain text without a specific structure.
    /// </summary>
    PlainText,
    /// <summary>
    /// Plain text with a known separator (e.g., CSV).
    /// </summary>
    PlainTextWithSeparator
}
