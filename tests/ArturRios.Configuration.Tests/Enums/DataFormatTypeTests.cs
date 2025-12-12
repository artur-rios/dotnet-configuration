using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

public class DataFormatTypeTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)DataFormatType.Json);
        Assert.Equal(1, (int)DataFormatType.ProtoBuf);
        Assert.Equal(2, (int)DataFormatType.Xml);
        Assert.Equal(3, (int)DataFormatType.PlainText);
        Assert.Equal(4, (int)DataFormatType.PlainTextWithSeparator);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Json", nameof(DataFormatType.Json));
        Assert.Equal("ProtoBuf", nameof(DataFormatType.ProtoBuf));
        Assert.Equal("Xml", nameof(DataFormatType.Xml));
        Assert.Equal("PlainText", nameof(DataFormatType.PlainText));
        Assert.Equal("PlainTextWithSeparator", nameof(DataFormatType.PlainTextWithSeparator));
    }
}
