using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArturRios.Configuration.Tests.Loaders;

[Trait("Category", "Functional")]
public class ConfigurationLoaderAppSettingsTests
{
    private static ILogger<ConfigurationLoader> Logger => NullLogger<ConfigurationLoader>.Instance;

    [Fact]
    public void GivenNoBuilderProvided_WhenLoadingAppSettings_ThenShouldThrow()
    {
        using var dir = new TestDirectory();

        var loader = new ConfigurationLoader("Local", dir.Root, Logger);

        Assert.Throws<InvalidOperationException>(() => loader.LoadAppSettings());
    }

    [Fact]
    public void GivenAppSettingsFile_WhenLoadingAppSettings_ThenShouldLoadAppSettings()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Settings");
        dir.WriteFile("Settings/appsettings.Local.json", "{\n  \"App\": { \"Name\": \"Sample\" }\n}");

        var builder = new ConfigurationBuilder();
        var loader = new ConfigurationLoader(builder, "Local", dir.Root, Logger);

        loader.LoadAppSettings();

        using var config = (ConfigurationRoot)builder.Build();

        Assert.Equal("Sample", config["App:Name"]);
    }

    [Fact]
    public void GivenMissingSpecificAppSettingsFile_WhenLoadingAppSettings_ThenShouldFallbackToDefault()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Settings");
        dir.WriteFile("Settings/appsettings.Local.json", "{\n  \"A\": \"1\"\n}");

        var builder = new ConfigurationBuilder();
        var loader = new ConfigurationLoader(builder, "Development", dir.Root, Logger);

        loader.LoadAppSettings();

        using var config = (ConfigurationRoot)builder.Build();

        Assert.Equal("1", config["A"]);
    }

    [Fact]
    public void GivenNoAppSettingsFiles_WhenLoadingAppSettings_ThenShouldNotLoadAppSettings()
    {
        using var dir = new TestDirectory();

        var builder = new ConfigurationBuilder();
        var loader = new ConfigurationLoader(builder, "Local", dir.Root, Logger);

        loader.LoadAppSettings();

        using var config = (ConfigurationRoot)builder.Build();

        Assert.Null(config["NonExistent:Key"]);
    }
    [Fact]
    public void GivenAnAppSettingsFile_WhenLoading_ThenItIsAddedAsARequiredJsonFileThatReloadsOnChange()
    {
        using var dir = new TestDirectory();

        dir.WriteFile("Settings/appsettings.Local.json", "{}");

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Local", dir.Root, Logger).LoadAppSettings();

        var source = Assert.IsType<JsonConfigurationSource>(Assert.Single(builder.Sources));
        Assert.False(source.Optional);
        Assert.True(source.ReloadOnChange);
    }

    [Fact]
    public void GivenSourcesAddedBeforeAndAfter_WhenLoadingAppSettings_ThenTheLaterSourceWins()
    {
        using var dir = new TestDirectory();

        dir.WriteFile("Settings/appsettings.Local.json", """{ "Before": "file", "After": "file" }""");

        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Before"] = "memory", ["After"] = "memory" });

        new ConfigurationLoader(builder, "Local", dir.Root, Logger).LoadAppSettings();

        builder.AddInMemoryCollection(new Dictionary<string, string?> { ["After"] = "override" });

        using var config = (ConfigurationRoot)builder.Build();

        Assert.Equal("file", config["Before"]);
        Assert.Equal("override", config["After"]);
    }

    [Fact]
    public void GivenAMalformedAppSettingsFile_WhenTheConfigurationIsBuilt_ThenItFails()
    {
        using var dir = new TestDirectory();

        dir.WriteFile("Settings/appsettings.Local.json", "{ \"App\": ");

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Local", dir.Root, Logger).LoadAppSettings();

        Assert.Throws<InvalidDataException>(() => builder.Build());
    }

    [Fact]
    public void GivenASettingsFolderWithOnlyOtherFiles_WhenLoading_ThenNothingIsAdded()
    {
        using var dir = new TestDirectory();

        dir.WriteFile("Settings/appsettings.json", "{}");
        dir.WriteFile("Settings/appsettings.Staging.json", "{}");

        var builder = new ConfigurationBuilder();

        new ConfigurationLoader(builder, "Development", dir.Root, Logger).LoadAppSettings();

        Assert.Empty(builder.Sources);
    }

    [Theory]
    [InlineData("Development", "Development", "Loading app settings for Development environment")]
    [InlineData("Local", "Development",
        "Could not find app settings for Development environment. " +
        "Loading default environment Local instead")]
    [InlineData(null, "Development", "Could not find any app settings")]
    public void GivenWhichFileExists_WhenLoading_ThenTheChoiceIsLoggedAsInformation(string? fileEnvironment,
        string environmentName, string expected)
    {
        using var dir = new TestDirectory();

        if (fileEnvironment is not null)
        {
            dir.WriteFile($"Settings/appsettings.{fileEnvironment}.json", "{}");
        }

        var logger = new RecordingLogger<ConfigurationLoader>();

        new ConfigurationLoader(new ConfigurationBuilder(), environmentName, dir.Root, logger).LoadAppSettings();

        var (level, message) = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, level);
        Assert.StartsWith(expected, message);
    }

    [Fact]
    public void GivenNoBasePathAndNoLogger_WhenLoading_ThenTheApplicationBaseDirectoryIsUsed()
    {
        // A file in the test's own output folder, under an environment name no other test uses.
        var environmentName = "Probe" + Guid.NewGuid().ToString("N");
        var file = Path.Combine(AppContext.BaseDirectory, "Settings", $"appsettings.{environmentName}.json");

        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, """{ "Probe": "base-directory" }""");

        try
        {
            var builder = new ConfigurationBuilder();

            new ConfigurationLoader(builder, environmentName).LoadAppSettings();

            using var config = (ConfigurationRoot)builder.Build();

            Assert.Equal("base-directory", config["Probe"]);
        }
        finally
        {
            File.Delete(file);
        }
    }
}
