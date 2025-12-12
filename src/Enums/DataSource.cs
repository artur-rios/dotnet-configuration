namespace ArturRios.Configuration.Enums;

/// <summary>
/// Indicates the type of data source used when persisting or retrieving configuration-related data.
/// </summary>
public enum DataSource
{
    /// <summary>
    /// A relational database source.
    /// </summary>
    Relational,
    /// <summary>
    /// A NoSQL database source.
    /// </summary>
    NoSql,
    /// <summary>
    /// An in-memory volatile store.
    /// </summary>
    InMemory
}
