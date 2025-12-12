using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

public class OutputTypeTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)OutputType.Void);
        Assert.Equal(1, (int)OutputType.Default);
        Assert.Equal(2, (int)OutputType.Primitive);
        Assert.Equal(3, (int)OutputType.Object);
        Assert.Equal(4, (int)OutputType.Exception);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Void", nameof(OutputType.Void));
        Assert.Equal("Default", nameof(OutputType.Default));
        Assert.Equal("Primitive", nameof(OutputType.Primitive));
        Assert.Equal("Object", nameof(OutputType.Object));
        Assert.Equal("Exception", nameof(OutputType.Exception));
    }
}
