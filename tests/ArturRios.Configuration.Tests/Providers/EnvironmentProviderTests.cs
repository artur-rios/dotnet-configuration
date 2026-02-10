using ArturRios.Configuration.Providers;
using ArturRios.Configuration.Tests.TestHelpers;

namespace ArturRios.Configuration.Tests.Providers;

public class EnvironmentProviderTests
{
    [Fact]
    public void GivenMissingEnvironmentVariable_WhenGettingValue_ThenReturnNull()
    {
        const string key = "MISSING_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, null);

        var provider = new EnvironmentProvider();

        Assert.Null(provider.GetBool(key));
        Assert.Null(provider.GetInt(key));
        Assert.Null(provider.GetString(key));
        Assert.Null(provider.GetObject<Person>(key));
    }

    [Fact]
    public void GivenBoolEnvironmentVariables_WhenParsingBool_ThenReturnCorrectValue()
    {
        const string keyTrue = "TRUE_KEY";
        const string keyFalse = "FALSE_KEY";

        using var scope = new EnvVarScope();

        scope.Set(keyTrue, "true");
        scope.Set(keyFalse, "false");

        var provider = new EnvironmentProvider();

        Assert.True(provider.GetBool(keyTrue));
        Assert.False(provider.GetBool(keyFalse));
    }

    [Fact]
    public void GivenInvalidBoolEnvironmentVariable_WhenParsingBool_ThenReturnNull()
    {
        const string key = "INVALID_BOOL_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, "not-a-bool");

        var provider = new EnvironmentProvider();

        Assert.Null(provider.GetBool(key));
    }

    [Fact]
    public void GivenIntEnvironmentVariable_WhenParsingInt_ThenReturnCorrectValue()
    {
        const string key = "INT_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, "123");

        var provider = new EnvironmentProvider();

        Assert.Equal(123, provider.GetInt(key));
    }

    [Fact]
    public void GivenInvalidIntEnvironmentVariable_WhenParsingInt_ThenReturnNull()
    {
        const string key = "INVALID_INT_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, "12x");

        var provider = new EnvironmentProvider();

        Assert.Null(provider.GetInt(key));
    }

    [Fact]
    public void GivenStringEnvironmentVariable_WhenGettingString_ThenReturnCorrectValue()
    {
        const string key = "STRING_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, "hello");

        var provider = new EnvironmentProvider();

        Assert.Equal("hello", provider.GetString(key));
    }

    [Fact]
    public void GivenJsonObjectEnvironmentVariable_WhenParsingObject_ThenReturnCorrectValue()
    {
        const string key = "VALID_OBJECT_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, "{\"Name\":\"John\",\"Age\":30}");

        var provider = new EnvironmentProvider();

        var person = provider.GetObject<Person>(key);

        Assert.NotNull(person);
        Assert.Equal("John", person.Name);
        Assert.Equal(30, person.Age);
    }

    [Fact]
    public void GivenInvalidJsonObject_WhenParsingObject_ThenReturnNull()
    {
        const string key = "INVALID_OBJECT_KEY";

        using var scope = new EnvVarScope();

        scope.Set(key, "{invalid-json");

        var provider = new EnvironmentProvider();

        Assert.Null(provider.GetObject<Person>(key));
    }
}
