using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

[Trait("Category", "Unit")]
public class OutputTypeTests
{
    [Fact]
    public void GivenOutputTypeEnum_WhenCheckingNumericValues_ThenShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)OutputType.Void);
        Assert.Equal(1, (int)OutputType.Default);
        Assert.Equal(2, (int)OutputType.Primitive);
        Assert.Equal(3, (int)OutputType.Object);
        Assert.Equal(4, (int)OutputType.Exception);
    }

    [Fact]
    public void GivenOutputTypeEnum_WhenCheckingNames_ThenShouldHaveExpectedNames()
    {
        // Every name, in value order: a member added, removed, renamed or moved fails the test. (nameof
        // compared a name with itself, so it could never fail.)
        Assert.Equal(
            new[] { "Void", "Default", "Primitive", "Object", "Exception" },
            Enum.GetNames<OutputType>());
    }
}
