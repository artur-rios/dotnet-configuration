using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ArturRios.Configuration.Tests.Loaders;

[Trait("Category", "Functional")]
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

        var loader = Loader("Local", dir);

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

        var loader = Loader("Development", dir);

        loader.LoadEnvironment();

        Assert.Equal("1", Environment.GetEnvironmentVariable("X"));
    }

    [Fact]
    public void GivenNoEnvFilesFound_WhenLoadingEnvironment_ThenShouldNotLoadEnvFile()
    {
        using var dir = new TestDirectory();
        using var scope = new EnvVarScope();

        scope.Set("Y", null);

        var loader = Loader("Local", dir);

        loader.LoadEnvironment();

        Assert.Null(Environment.GetEnvironmentVariable("Y"));
    }

    // The real environment wins over .env files: a variable the deployment injected must not be overwritten by a
    // .env file shipped with the build — least of all by the .env.local fallback, which is loaded for any
    // environment that has no file of its own, Production included.

    private static ConfigurationLoader Loader(string environmentName, TestDirectory dir) =>
        new(environmentName, dir.Root, NullLogger<ConfigurationLoader>.Instance);

    [Fact]
    public void GivenAVariableAlreadySet_WhenTheEnvironmentFileDefinesIt_ThenTheExistingValueIsKept()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.production", "CFG_TEST_DB=from-file\nCFG_TEST_ONLY_IN_FILE=file-value");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_DB", "from-deployment");
        scope.Set("CFG_TEST_ONLY_IN_FILE", null);

        Loader("Production", dir).LoadEnvironment();

        Assert.Equal("from-deployment", Environment.GetEnvironmentVariable("CFG_TEST_DB"));
        Assert.Equal("file-value", Environment.GetEnvironmentVariable("CFG_TEST_ONLY_IN_FILE"));
    }

    [Fact]
    public void GivenAVariableAlreadySet_WhenFallingBackToTheLocalFile_ThenTheExistingValueIsKept()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.local", "CFG_TEST_DB=localhost\nCFG_TEST_ONLY_IN_FILE=file-value");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_DB", "prod-db");
        scope.Set("CFG_TEST_ONLY_IN_FILE", null);

        Loader("Production", dir).LoadEnvironment();

        Assert.Equal("prod-db", Environment.GetEnvironmentVariable("CFG_TEST_DB"));
        Assert.Equal("file-value", Environment.GetEnvironmentVariable("CFG_TEST_ONLY_IN_FILE"));
    }

    [Fact]
    public void GivenAnEnvironmentFileAndALocalFile_WhenLoading_ThenOnlyTheEnvironmentFileIsUsed()
    {
        // The precedence among the files themselves is unchanged: the environment's own file is loaded and the
        // local file is only a fallback, never layered underneath it.
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.development", "CFG_TEST_DB=dev-db");
        dir.WriteFile("Environments/.env.local", "CFG_TEST_DB=localhost\nCFG_TEST_LOCAL_ONLY=local");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_DB", null);
        scope.Set("CFG_TEST_LOCAL_ONLY", null);

        Loader("Development", dir).LoadEnvironment();

        Assert.Equal("dev-db", Environment.GetEnvironmentVariable("CFG_TEST_DB"));
        Assert.Null(Environment.GetEnvironmentVariable("CFG_TEST_LOCAL_ONLY"));
    }

    [Fact]
    public void GivenAKeyRepeatedInTheFile_WhenLoading_ThenTheFirstOccurrenceWins()
    {
        // DotNetEnv's no-clobber mode: once the first occurrence has set the variable, the later ones find it
        // already set. Pinned so a change to it is a deliberate one (it was last-wins before 1.3.0).
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.local", "CFG_TEST_DB=first\nCFG_TEST_DB=second");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_DB", null);

        Loader("Local", dir).LoadEnvironment();

        Assert.Equal("first", Environment.GetEnvironmentVariable("CFG_TEST_DB"));
    }

    [Fact]
    public void GivenAFileValueInterpolatingAVariableAlreadySet_WhenLoading_ThenTheExistingValueIsInterpolated()
    {
        using var dir = new TestDirectory();

        dir.EnsureSubfolder("Environments");
        dir.WriteFile("Environments/.env.local", "CFG_TEST_HOST=localhost\nCFG_TEST_URL=https://${CFG_TEST_HOST}/api");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_HOST", "prod.example.com");
        scope.Set("CFG_TEST_URL", null);

        Loader("Local", dir).LoadEnvironment();

        Assert.Equal("prod.example.com", Environment.GetEnvironmentVariable("CFG_TEST_HOST"));
        Assert.Equal("https://prod.example.com/api", Environment.GetEnvironmentVariable("CFG_TEST_URL"));
    }
    [Fact]
    public void GivenAnEnvironmentsFolderWithOnlyOtherFiles_WhenLoading_ThenNothingIsLoaded()
    {
        using var dir = new TestDirectory();

        dir.WriteFile("Environments/.env", "CFG_TEST_OTHER=root");
        dir.WriteFile("Environments/.env.staging", "CFG_TEST_OTHER=staging");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_OTHER", null);

        Loader("Development", dir).LoadEnvironment();

        Assert.Null(Environment.GetEnvironmentVariable("CFG_TEST_OTHER"));
    }

    [Fact]
    public void GivenAMalformedEnvironmentFile_WhenLoading_ThenTheParseErrorIsThrown()
    {
        using var dir = new TestDirectory();

        dir.WriteFile("Environments/.env.local", "CFG_TEST_DB=\"never closed");

        using var scope = new EnvVarScope();

        scope.Set("CFG_TEST_DB", null);

        // The parser's own exception (Superpower.ParseException, through DotNetEnv) is not caught or wrapped, and no
        // variable of the file is set.
        Assert.ThrowsAny<Exception>(() => Loader("Local", dir).LoadEnvironment());
        Assert.Null(Environment.GetEnvironmentVariable("CFG_TEST_DB"));
    }

    [Theory]
    [InlineData("Development", "Development", "Loading variables for Development environment")]
    [InlineData("local", "Development",
        "Could not find variables for Development environment. " +
        "Loading default environment Local instead")]
    [InlineData(null, "Development", "Could not find any environment variables")]
    public void GivenWhichFileExists_WhenLoading_ThenTheChoiceIsLoggedAsInformation(string? fileEnvironment,
        string environmentName, string expected)
    {
        using var dir = new TestDirectory();

        if (fileEnvironment is not null)
        {
            dir.WriteFile($"Environments/.env.{fileEnvironment}", string.Empty);
        }

        var logger = new RecordingLogger<ConfigurationLoader>();

        new ConfigurationLoader(environmentName, dir.Root, logger).LoadEnvironment();

        var (level, message) = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, level);
        Assert.StartsWith(expected, message);
    }

    [Fact]
    public void GivenNoBasePathAndNoLogger_WhenLoading_ThenTheApplicationBaseDirectoryIsUsed()
    {
        // A file in the test's own output folder, under an environment name and a key no other test uses.
        var environmentName = "Probe" + Guid.NewGuid().ToString("N");
        var key = "ARTURRIOS_CFG_" + Guid.NewGuid().ToString("N");
        var file = Path.Combine(AppContext.BaseDirectory, "Environments", $".env.{environmentName}");

        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, $"{key}=base-directory");

        using var scope = new EnvVarScope();

        scope.Set(key, null);

        try
        {
            new ConfigurationLoader(environmentName).LoadEnvironment();

            Assert.Equal("base-directory", Environment.GetEnvironmentVariable(key));
        }
        finally
        {
            File.Delete(file);
        }
    }
}
