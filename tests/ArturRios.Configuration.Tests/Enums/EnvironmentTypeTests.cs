using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

[Trait("Category", "Unit")]
public class EnvironmentTypeTests
{
    [Fact]
    public void GivenEnvironmentTypeEnum_WhenCheckingNumericValues_ThenShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)EnvironmentType.Local);
        Assert.Equal(1, (int)EnvironmentType.Development);
        Assert.Equal(2, (int)EnvironmentType.Staging);
        Assert.Equal(3, (int)EnvironmentType.Production);
    }

    [Fact]
    public void GivenEnvironmentTypeEnum_WhenCheckingNames_ThenShouldHaveExpectedNames()
    {
        // Every name, in value order: a member added, removed, renamed or moved fails the test. (nameof
        // compared a name with itself, so it could never fail.)
        Assert.Equal(
            new[] { "Local", "Development", "Staging", "Production" },
            Enum.GetNames<EnvironmentType>());
    }
}
