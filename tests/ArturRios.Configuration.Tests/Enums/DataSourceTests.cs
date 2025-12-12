using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

public class DataSourceTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)DataSource.Relational);
        Assert.Equal(1, (int)DataSource.NoSql);
        Assert.Equal(2, (int)DataSource.InMemory);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Relational", nameof(DataSource.Relational));
        Assert.Equal("NoSql", nameof(DataSource.NoSql));
        Assert.Equal("InMemory", nameof(DataSource.InMemory));
    }
}
