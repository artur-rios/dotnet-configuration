using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

public class DataSourceTests
{
    [Fact]
    public void GivenDataSourceEnum_WhenCheckingNumericValues_ThenShouldHaveExpectedValues()
    {
        Assert.Equal(0, (int)DataSource.Relational);
        Assert.Equal(1, (int)DataSource.NoSql);
        Assert.Equal(2, (int)DataSource.InMemory);
    }

    [Fact]
    public void GivenDataSourceEnum_WhenCheckingNames_ThenShouldHaveExpectedNames()
    {
        Assert.Equal("Relational", nameof(DataSource.Relational));
        Assert.Equal("NoSql", nameof(DataSource.NoSql));
        Assert.Equal("InMemory", nameof(DataSource.InMemory));
    }
}
