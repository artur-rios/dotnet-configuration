using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArturRios.Configuration.Tests.Loaders;

public class ConfigurationLoaderAppSettingsTests
{
    [Fact]
    public void GivenNoBuilderProvided_WhenLoadingAppSettings_ThenShouldThrow()
    {
        using var dir = new TestDirectory();

        var logger = LoggerFactory.Create(b => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader("Local", dir.Root, logger);

        Assert.Throws<InvalidOperationException>(() => loader.LoadAppSettings());
    }

    [Fact]
    public void GivenAppSettingsFile_WhenLoadingAppSettings_ThenShouldLoadAppSettings()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Settings");
        dir.WriteFile("Settings/appsettings.Local.json", "{\n  \"App\": { \"Name\": \"Sample\" }\n}");

        var builder = new ConfigurationBuilder();
        var logger = LoggerFactory.Create(b => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader(builder, "Local", dir.Root, logger);

        loader.LoadAppSettings();

        var config = builder.Build();

        Assert.Equal("Sample", config["App:Name"]);
    }

    [Fact]
    public void GivenMissingSpecificAppSettingsFile_WhenLoadingAppSettings_ThenShouldFallbackToDefault()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Settings");
        dir.WriteFile("Settings/appsettings.Local.json", "{\n  \"A\": \"1\"\n}");

        var builder = new ConfigurationBuilder();
        var logger = LoggerFactory.Create(b => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader(builder, "Development", dir.Root, logger);

        loader.LoadAppSettings();

        var config = builder.Build();

        Assert.Equal("1", config["A"]);
    }

    [Fact]
    public void GivenNoAppSettingsFiles_WhenLoadingAppSettings_ThenShouldNotLoadAppSettings()
    {
        using var dir = new TestDirectory();

        var builder = new ConfigurationBuilder();
        var logger = LoggerFactory.Create(b => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader(builder, "Local", dir.Root, logger);

        loader.LoadAppSettings();

        var config = builder.Build();

        Assert.Null(config["NonExistent:Key"]);
    }
}
