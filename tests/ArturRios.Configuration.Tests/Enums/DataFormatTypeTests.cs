using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

[Trait("Category", "Unit")]
public class DataFormatTypeTests
{
    [Fact]
    public void GivenDataFormatTypeEnum_WhenCheckingNumericValues_ThenShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)DataFormatType.Json);
        Assert.Equal(1, (int)DataFormatType.ProtoBuf);
        Assert.Equal(2, (int)DataFormatType.Xml);
        Assert.Equal(3, (int)DataFormatType.PlainText);
        Assert.Equal(4, (int)DataFormatType.PlainTextWithSeparator);
    }

    [Fact]
    public void GivenDataFormatTypeEnum_WhenCheckingNames_ThenShouldHaveExpectedNames()
    {
        // Every name, in value order: a member added, removed, renamed or moved fails the test. (nameof
        // compared a name with itself, so it could never fail.)
        Assert.Equal(
            new[] { "Json", "ProtoBuf", "Xml", "PlainText", "PlainTextWithSeparator" },
            Enum.GetNames<DataFormatType>());
    }
}
