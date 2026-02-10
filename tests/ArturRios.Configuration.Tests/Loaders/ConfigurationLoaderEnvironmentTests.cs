using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace ArturRios.Configuration.Tests.Loaders;

public class ConfigurationLoaderEnvironmentTests
{
    [Fact]
    public void GivenEnvironmentFile_WhenLoadingEnvironment_ThenShouldLoadEnvFile()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.local", "HELLO=WORLD");

        using var scope = new EnvVarScope();

        scope.Set("HELLO", null);

        var logger = LoggerFactory.Create(_ => { }).CreateLogger<ConfigurationLoader>();

        var loader = new ConfigurationLoader("Local", dir.Root, logger);

        loader.LoadEnvironment();

        Assert.Equal("WORLD", Environment.GetEnvironmentVariable("HELLO"));
    }

    [Fact]
    public void GivenMissingSpecificEnvFile_WhenLoadingEnvironment_ThenShouldFallbackToDefault()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.local", "X=1");

        using var scope = new EnvVarScope();

        scope.Set("X", null);

        var logger = LoggerFactory.Create(_ => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader("Development", dir.Root, logger);

        loader.LoadEnvironment();

        Assert.Equal("1", Environment.GetEnvironmentVariable("X"));
    }

    [Fact]
    public void GivenNoEnvFilesFound_WhenLoadingEnvironment_ThenShouldNotLoadEnvFile()
    {
        using var dir = new TestDirectory();
        using var scope = new EnvVarScope();

        scope.Set("Y", null);

        var logger = LoggerFactory.Create(_ => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader("Local", dir.Root, logger);

        loader.LoadEnvironment();

        Assert.Null(Environment.GetEnvironmentVariable("Y"));
    }
}
