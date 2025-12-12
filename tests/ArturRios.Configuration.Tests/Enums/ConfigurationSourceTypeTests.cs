using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

public class ConfigurationSourceTypeTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)ConfigurationSourceType.EnvFile);
        Assert.Equal(1, (int)ConfigurationSourceType.AppSettings);
        Assert.Equal(2, (int)ConfigurationSourceType.EnvironmentVariables);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("EnvFile", nameof(ConfigurationSourceType.EnvFile));
        Assert.Equal("AppSettings", nameof(ConfigurationSourceType.AppSettings));
        Assert.Equal("EnvironmentVariables", nameof(ConfigurationSourceType.EnvironmentVariables));
    }
}
