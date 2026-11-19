using ArturRios.Configuration.Enums;

namespace ArturRios.Configuration.Tests.Enums;

[Trait("Category", "Unit")]
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
        // Every name, in value order: a member added, removed, renamed or moved fails the test. (nameof
        // compared a name with itself, so it could never fail.)
        Assert.Equal(
            new[] { "Relational", "NoSql", "InMemory" },
            Enum.GetNames<DataSource>());
    }
}
