using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArturRios.Configuration.Tests.Loaders;

public class ConfigurationLoaderAppSettingsTests
{
    [Fact]
    public void Should_Throw_When_BuilderNotProvided()
    {
        using var dir = new TestDirectory();

        var logger = LoggerFactory.Create(b => { }).CreateLogger<ConfigurationLoader>();
        var loader = new ConfigurationLoader("Local", dir.Root, logger);

        Assert.Throws<InvalidOperationException>(() => loader.LoadAppSettings());
    }

    [Fact]
    public void Should_LoadAppsettings()
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
    public void Should_FallbackToDefaultAppSettings_WhenSpecificIsMissing()
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
    public void ShouldNot_LoadAppSettingsWhenNoAppSettingsFilesFound()
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
