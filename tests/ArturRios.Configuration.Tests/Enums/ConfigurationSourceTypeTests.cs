using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

[Trait("Category", "Unit")]
public class ConfigurationSourceTypeTests
{
    [Fact]
    public void GivenConfigurationSourceTypeEnum_WhenCheckingNumericValues_ThenShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)ConfigurationSourceType.EnvFile);
        Assert.Equal(1, (int)ConfigurationSourceType.AppSettings);
        Assert.Equal(2, (int)ConfigurationSourceType.EnvironmentVariables);
    }

    [Fact]
    public void GivenConfigurationSourceTypeEnum_WhenCheckingNames_ThenShouldHaveExpectedNames()
    {
        // Every name, in value order: a member added, removed, renamed or moved fails the test. (nameof
        // compared a name with itself, so it could never fail.)
        Assert.Equal(
            new[] { "EnvFile", "AppSettings", "EnvironmentVariables" },
            Enum.GetNames<ConfigurationSourceType>());
    }
}
