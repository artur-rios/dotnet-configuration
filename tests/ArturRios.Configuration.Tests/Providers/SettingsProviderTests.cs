using ArturRios.Configuration.Providers;
using ArturRios.Configuration.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;

namespace ArturRios.Configuration.Tests.Providers;

public class SettingsProviderTests
{
    private static IConfiguration BuildConfig(params (string Key, string? Value)[] items)
    {
        var dict = new Dictionary<string, string?>();

        foreach (var (key, value) in items)
        {
            dict[key] = value;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();
    }

    [Fact]
    public void Should_ReturnNullWhenKeyMissing()
    {
        var provider = new SettingsProvider(BuildConfig());

        Assert.Null(provider.GetBool("missing"));
        Assert.Null(provider.GetInt("missing"));
        Assert.Null(provider.GetString("missing"));
        Assert.Null(provider.GetObject<Person>("missing"));
    }

    [Fact]
    public void Should_ParseBoolFromSettings()
    {
        var provider = new SettingsProvider(BuildConfig(("featureA", "true"), ("featureB", "false")));

        Assert.True(provider.GetBool("featureA"));
        Assert.False(provider.GetBool("featureB"));
    }

    [Fact]
    public void Should_ReturnNullForInvalidBoolValue()
    {
        var provider = new SettingsProvider(BuildConfig(("flag", "maybe")));
        Assert.Null(provider.GetBool("flag"));
    }

    [Fact]
    public void Should_ParseIntFromSettings()
    {
        var provider = new SettingsProvider(BuildConfig(("port", "8080")));

        Assert.Equal(8080, provider.GetInt("port"));
    }

    [Fact]
    public void Should_ReturnNullForInvalidInt()
    {
        var provider = new SettingsProvider(BuildConfig(("port", "eighty")));

        Assert.Null(provider.GetInt("port"));
    }

    [Fact]
    public void Should_GetStringFromSettings()
    {
        var provider = new SettingsProvider(BuildConfig(("greeting", "hello")));
        Assert.Equal("hello", provider.GetString("greeting"));
    }

    [Fact]
    public void Should_ParseObjectFromSettings()
    {
        var provider = new SettingsProvider(BuildConfig(("user", "{\"Name\":\"Ana\",\"Age\":25}")));
        var person = provider.GetObject<Person>("user");
        Assert.NotNull(person);
        Assert.Equal("Ana", person.Name);
        Assert.Equal(25, person.Age);
    }

    [Fact]
    public void Should_ReturnNullWhenObjectIsInvalid()
    {
        var provider = new SettingsProvider(BuildConfig(("user", "{bad-json")));

        Assert.Null(provider.GetObject<Person>("user"));
    }
}
