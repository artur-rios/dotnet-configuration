using ArturRios.Configuration.Loaders;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

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

    // The real environment wins over .env files: a variable the deployment injected must not be overwritten by a
    // .env file shipped with the build — least of all by the .env.local fallback, which is loaded for any
    // environment that has no file of its own, Production included.

    private static ConfigurationLoader Loader(string environmentName, TestDirectory dir) =>
        new(environmentName, dir.Root, LoggerFactory.Create(_ => { }).CreateLogger<ConfigurationLoader>());

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
}
