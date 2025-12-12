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
/// By default, it looks for files under the <c>Environments</c> and <c>Settings</c> folders within the application base path.
/// If a specific environment file is not found, it falls back to the <see cref="EnvironmentType.Local"/> environment.
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
    /// <param name="logger">Optional logger instance. If not provided, a console logger is created.</param>
    public ConfigurationLoader(IConfigurationBuilder configurationBuilder, string environmentName,
        string? basePath = null, ILogger<ConfigurationLoader>? logger = null)
    {
        _configurationBuilder = configurationBuilder;
        _environmentName = environmentName;
        _basePath = string.IsNullOrEmpty(basePath) ? AppDomain.CurrentDomain.BaseDirectory : basePath;
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ConfigurationLoader>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigurationLoader"/> without a configuration builder.
    /// </summary>
    /// <param name="environmentName">The environment name (e.g., <c>Local</c>, <c>Development</c>, <c>Staging</c>, <c>Production</c>).</param>
    /// <param name="basePath">Optional base path to search for files. If not provided, the current domain base directory is used.</param>
    /// <param name="logger">Optional logger instance. If not provided, a console logger is created.</param>
    /// <remarks>
    /// Use <see cref="LoadEnvironment"/> with this constructor. <see cref="LoadAppSettings"/> requires an <see cref="IConfigurationBuilder"/>.
    /// </remarks>
    public ConfigurationLoader(string environmentName, string? basePath = null,
        ILogger<ConfigurationLoader>? logger = null)
    {
        _environmentName = environmentName;
        _basePath = string.IsNullOrEmpty(basePath) ? AppDomain.CurrentDomain.BaseDirectory : basePath;
        _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ConfigurationLoader>();
    }

    /// <summary>
    /// Loads environment variables from a specific <c>.env</c> file based on the configured environment.
    /// </summary>
    /// <remarks>
    /// It looks for <c>Environments/.env.&lt;environment&gt;</c> and falls back to <c>Environments/.env.local</c> if not found.
    /// </remarks>
    public void LoadEnvironment()
    {
        var envFolder = Path.Combine(_basePath, DefaultEnvFileFolder);
        var envFile = Path.Combine(envFolder, $".env.{_environmentName.ToLower()}");
        var defaultEnvFile = Path.Combine(envFolder, $".env.{DefaultEnvironmentName.ToLower()}");

        if (File.Exists(envFile))
        {
            _logger.LogInformation("Loading variables for {EnvironmentName} environment...", _environmentName);

            Env.Load(envFile);
        }
        else if (File.Exists(defaultEnvFile))
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
    /// </remarks>
    public void LoadAppSettings()
    {
        var settingsFolder = Path.Combine(_basePath, DefaultAppSettingsFolder);
        var envSettingsFile = Path.Combine(settingsFolder, $"appsettings.{_environmentName}.json");
        var defaultSettingsFile = Path.Combine(settingsFolder, $"appsettings.{DefaultEnvironmentName}.json");

        if (_configurationBuilder is null)
        {
            throw new InvalidOperationException(
                "Cannot load appsettings.json if configuration builder is not provided on constructor");
        }

        if (File.Exists(envSettingsFile))
        {
            _logger.LogInformation("Loading app settings for {EnvironmentName} environment...", _environmentName);

            _configurationBuilder.AddJsonFile(envSettingsFile, false, true);
        }
        else if (File.Exists(defaultSettingsFile))
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
}
