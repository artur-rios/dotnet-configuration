using ArturRios.Configuration.Enums;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArturRios.Configuration.Loaders;

/// <summary>
/// Provides helper methods to load environment variables from <c>.env</c> files and
/// application settings from <c>appsettings.&lt;Environment&gt;.json</c> files.
/// </summary>
/// <remarks>
/// <para>
/// By default, it looks for files under the <c>Environments</c> and <c>Settings</c> folders within the application base path.
/// If a specific environment file is not found, it falls back to the <see cref="EnvironmentType.Local"/> environment.
/// </para>
/// <para>
/// File names are matched without regard to case, so <c>.env.Development</c> and <c>.env.development</c> —
/// or <c>appsettings.Local.json</c> and <c>appsettings.local.json</c> — are equally acceptable on every
/// platform. Matching by exact case only would resolve on Windows and silently find nothing on Linux.
/// </para>
/// </remarks>
public class ConfigurationLoader
{
    /// <summary>
    /// Default environment name used when a specific environment file cannot be found.
    /// </summary>
    private const string DefaultEnvironmentName = nameof(EnvironmentType.Local);
    /// <summary>
    /// Default folder name that contains <c>.env</c> files.
    /// </summary>
    private const string DefaultEnvFileFolder = "Environments";
    /// <summary>
    /// Default folder name that contains <c>appsettings</c> files.
    /// </summary>
    private const string DefaultAppSettingsFolder = "Settings";

    /// <summary>
    /// One console logger factory for the whole process. Creating a factory per loader leaked one provider
    /// per instance, since nothing ever disposed them.
    /// </summary>
    private static readonly Lazy<ILoggerFactory> FallbackLoggerFactory =
        new(() => LoggerFactory.Create(builder => builder.AddConsole()), isThreadSafe: true);

    private readonly string _basePath;
    private readonly IConfigurationBuilder? _configurationBuilder;
    private readonly string _environmentName;
    private readonly ILogger<ConfigurationLoader> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationLoader"/> using an existing <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder where JSON files will be added.</param>
    /// <param name="environmentName">The environment name (e.g., <c>Local</c>, <c>Development</c>, <c>Staging</c>, <c>Production</c>).</param>
    /// <param name="basePath">Optional base path to search for files. If not provided, the current domain base directory is used.</param>
    /// <param name="logger">Optional logger instance. If not provided, a shared console logger is used.</param>
    /// <exception cref="ArgumentNullException"><paramref name="configurationBuilder"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="environmentName"/> is <c>null</c> or whitespace.</exception>
    public ConfigurationLoader(IConfigurationBuilder configurationBuilder, string environmentName,
        string? basePath = null, ILogger<ConfigurationLoader>? logger = null)
        : this(environmentName, basePath, logger)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        _configurationBuilder = configurationBuilder;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationLoader"/> without a configuration builder.
    /// </summary>
    /// <param name="environmentName">The environment name (e.g., <c>Local</c>, <c>Development</c>, <c>Staging</c>, <c>Production</c>).</param>
    /// <param name="basePath">Optional base path to search for files. If not provided, the current domain base directory is used.</param>
    /// <param name="logger">Optional logger instance. If not provided, a shared console logger is used.</param>
    /// <exception cref="ArgumentException"><paramref name="environmentName"/> is <c>null</c> or whitespace.</exception>
    /// <remarks>
    /// Use <see cref="LoadEnvironment"/> with this constructor. <see cref="LoadAppSettings"/> requires an <see cref="IConfigurationBuilder"/>.
    /// </remarks>
    public ConfigurationLoader(string environmentName, string? basePath = null,
        ILogger<ConfigurationLoader>? logger = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environmentName);

        _environmentName = environmentName;
        _basePath = string.IsNullOrEmpty(basePath) ? AppDomain.CurrentDomain.BaseDirectory : basePath;
        _logger = logger ?? FallbackLoggerFactory.Value.CreateLogger<ConfigurationLoader>();
    }

    /// <summary>
    /// Loads environment variables from a specific <c>.env</c> file based on the configured environment.
    /// </summary>
    /// <remarks>
    /// It looks for <c>Environments/.env.&lt;environment&gt;</c> and falls back to <c>Environments/.env.local</c> if not found.
    /// The file name is matched without regard to case.
    /// </remarks>
    public void LoadEnvironment()
    {
        var envFolder = Path.Combine(_basePath, DefaultEnvFileFolder);
        var envFile = ResolveFile(envFolder, $".env.{_environmentName}");
        var defaultEnvFile = ResolveFile(envFolder, $".env.{DefaultEnvironmentName}");

        if (envFile is not null)
        {
            _logger.LogInformation("Loading variables for {EnvironmentName} environment...", _environmentName);

            Env.Load(envFile);
        }
        else if (defaultEnvFile is not null)
        {
            _logger.LogInformation(
                "Could not find variables for {EnvironmentName} environment. Loading default environment {DefaultEnvironmentName} instead...",
                _environmentName, DefaultEnvironmentName);

            Env.Load(defaultEnvFile);
        }
        else
        {
            _logger.LogInformation("Could not find any environment variables");
        }
    }

    /// <summary>
    /// Loads JSON application settings for the configured environment into the provided <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no <see cref="IConfigurationBuilder"/> was provided in the constructor.</exception>
    /// <remarks>
    /// It looks for <c>Settings/appsettings.&lt;environment&gt;.json</c> and falls back to <c>Settings/appsettings.local.json</c> if not found.
    /// The file name is matched without regard to case.
    /// </remarks>
    public void LoadAppSettings()
    {
        if (_configurationBuilder is null)
        {
            throw new InvalidOperationException(
                "Cannot load appsettings.json if configuration builder is not provided on constructor");
        }

        var settingsFolder = Path.Combine(_basePath, DefaultAppSettingsFolder);
        var envSettingsFile = ResolveFile(settingsFolder, $"appsettings.{_environmentName}.json");
        var defaultSettingsFile = ResolveFile(settingsFolder, $"appsettings.{DefaultEnvironmentName}.json");

        if (envSettingsFile is not null)
        {
            _logger.LogInformation("Loading app settings for {EnvironmentName} environment...", _environmentName);

            _configurationBuilder.AddJsonFile(envSettingsFile, false, true);
        }
        else if (defaultSettingsFile is not null)
        {
            _logger.LogInformation(
                "Could not find app settings for {EnvironmentName} environment. Loading default environment {DefaultEnvironmentName} instead...",
                _environmentName, DefaultEnvironmentName);

            _configurationBuilder.AddJsonFile(defaultSettingsFile, false, true);
        }
        else
        {
            _logger.LogInformation("Could not find any app settings");
        }
    }

    /// <summary>
    /// Finds <paramref name="fileName"/> inside <paramref name="folder"/>, ignoring case.
    /// </summary>
    /// <returns>The full path of the matching file, or <c>null</c> when the folder or the file is absent.</returns>
    /// <remarks>
    /// The exact-case path is probed first, which is the common case and costs a single stat call. Only when
    /// that misses is the directory enumerated, so a case-insensitive file system never pays for the scan.
    /// </remarks>
    private static string? ResolveFile(string folder, string fileName)
    {
        var exact = Path.Combine(folder, fileName);

        if (File.Exists(exact))
        {
            return exact;
        }

        if (!Directory.Exists(folder))
        {
            return null;
        }

        foreach (var candidate in Directory.EnumerateFiles(folder))
        {
            if (string.Equals(Path.GetFileName(candidate), fileName, StringComparison.OrdinalIgnoreCase))
            {
                return candidate;
            }
        }

        return null;
    }
}
