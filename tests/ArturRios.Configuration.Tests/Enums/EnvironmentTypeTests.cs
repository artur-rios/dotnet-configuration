using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

public class EnvironmentTypeTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)EnvironmentType.Local);
        Assert.Equal(1, (int)EnvironmentType.Development);
        Assert.Equal(2, (int)EnvironmentType.Staging);
        Assert.Equal(3, (int)EnvironmentType.Production);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Local", nameof(EnvironmentType.Local));
        Assert.Equal("Development", nameof(EnvironmentType.Development));
        Assert.Equal("Staging", nameof(EnvironmentType.Staging));
        Assert.Equal("Production", nameof(EnvironmentType.Production));
    }
}
