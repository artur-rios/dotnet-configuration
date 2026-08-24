using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArturRios.Configuration.Tests.Loaders;

/// <summary>
/// File names are resolved without regard to case. Matching on exact case only resolved on Windows and
/// silently found nothing on a case-sensitive file system, which is what CI and most deployments run on.
/// </summary>
[Trait("Category", "Functional")]
public class ConfigurationLoaderFileNameCasingTests
{
    private static ILogger<ConfigurationLoader> Logger => NullLogger<ConfigurationLoader>.Instance;

    [Theory]
    [InlineData("appsettings.local.json")]
    [InlineData("appsettings.Local.json")]
    [InlineData("appsettings.LOCAL.json")]
    public void GivenTheDefaultSettingsFileInAnyCasing_WhenFallingBack_ThenItIsFound(string fileName)
    {
        using var directory = new TestDirectory();

        directory.WriteFile($"Settings/{fileName}", """{ "A": "fallback" }""");

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Development", directory.Root, Logger).LoadAppSettings();

        Assert.Equal("fallback", builder.Build()["A"]);
    }

    [Theory]
    [InlineData("appsettings.development.json")]
    [InlineData("appsettings.Development.json")]
    [InlineData("appsettings.DEVELOPMENT.json")]
    public void GivenAnEnvironmentSettingsFileInAnyCasing_WhenLoading_ThenItIsFound(string fileName)
    {
        using var directory = new TestDirectory();

        directory.WriteFile($"Settings/{fileName}", """{ "A": "environment" }""");

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Development", directory.Root, Logger).LoadAppSettings();

        Assert.Equal("environment", builder.Build()["A"]);
    }

    [Fact]
    public void GivenBothAnEnvironmentAndADefaultSettingsFile_WhenLoading_ThenTheEnvironmentOneWins()
    {
        using var directory = new TestDirectory();

        directory.WriteFile("Settings/appsettings.local.json", """{ "A": "fallback" }""");
        directory.WriteFile("Settings/appsettings.Development.json", """{ "A": "environment" }""");

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Development", directory.Root, Logger).LoadAppSettings();

        Assert.Equal("environment", builder.Build()["A"]);
    }

    [Fact]
    public void GivenNoSettingsFolderAtAll_WhenLoading_ThenNothingIsAddedAndNothingThrows()
    {
        using var directory = new TestDirectory();

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Development", directory.Root, Logger).LoadAppSettings();

        Assert.Empty(builder.Sources);
    }

    [Theory]
    [InlineData(".env.local")]
    [InlineData(".env.Local")]
    [InlineData(".env.LOCAL")]
    public void GivenTheDefaultEnvFileInAnyCasing_WhenFallingBack_ThenItIsFound(string fileName)
    {
        var key = "ARTURRIOS_CFG_" + Guid.NewGuid().ToString("N");

        using var directory = new TestDirectory();
        using var environment = new EnvVarScope();

        environment.Set(key, null);
        directory.WriteFile($"Environments/{fileName}", $"{key}=fallback");

        new ConfigurationLoader("Development", directory.Root, Logger).LoadEnvironment();

        Assert.Equal("fallback", Environment.GetEnvironmentVariable(key));
    }

    [Theory]
    [InlineData(".env.development")]
    [InlineData(".env.Development")]
    public void GivenAnEnvironmentEnvFileInAnyCasing_WhenLoading_ThenItIsFound(string fileName)
    {
        var key = "ARTURRIOS_CFG_" + Guid.NewGuid().ToString("N");

        using var directory = new TestDirectory();
        using var environment = new EnvVarScope();

        environment.Set(key, null);
        directory.WriteFile($"Environments/{fileName}", $"{key}=environment");

        new ConfigurationLoader("Development", directory.Root, Logger).LoadEnvironment();

        Assert.Equal("environment", Environment.GetEnvironmentVariable(key));
    }

    [Fact]
    public void GivenNoEnvironmentsFolderAtAll_WhenLoading_ThenNothingThrows()
    {
        using var directory = new TestDirectory();

        new ConfigurationLoader("Development", directory.Root, Logger).LoadEnvironment();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void GivenNoEnvironmentName_WhenConstructing_ThenArgumentExceptionIsThrown(string? environmentName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ConfigurationLoader(environmentName!));
        Assert.ThrowsAny<ArgumentException>(
            () => new ConfigurationLoader(new ConfigurationBuilder(), environmentName!));
    }

    [Fact]
    public void GivenNoConfigurationBuilder_WhenConstructing_ThenArgumentNullExceptionIsThrown()
    {
        Assert.Throws<ArgumentNullException>(() => new ConfigurationLoader((IConfigurationBuilder)null!, "Local"));
    }
}
