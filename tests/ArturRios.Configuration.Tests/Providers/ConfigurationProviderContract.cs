using ArturRios.Configuration.Tests.TestHelpers;
using IConfigurationProvider = ArturRios.Configuration.Providers.Interfaces.IConfigurationProvider;

namespace ArturRios.Configuration.Tests.Providers;

/// <summary>
///     What every <see cref="IConfigurationProvider" /> of the package does with a raw value, whatever it reads it
///     from: each provider's test class derives from this one and says how a key gets its value.
/// </summary>
public abstract class ConfigurationProviderContract : IDisposable
{
    /// <summary>A provider in which <paramref name="key" /> holds <paramref name="value" />.</summary>
    protected abstract IConfigurationProvider ProviderWith(string key, string value);

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private static string NewKey() => "ARTURRIOS_CFG_" + Guid.NewGuid().ToString("N");

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("TRUE", true)]
    [InlineData(" true ", true)]
    [InlineData("false", false)]
    [InlineData("FALSE", false)]
    public void GivenABooleanInAnyCasing_WhenParsingBool_ThenItIsRead(string value, bool expected)
    {
        var key = NewKey();

        Assert.Equal(expected, ProviderWith(key, value).GetBool(key));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("yes")]
    [InlineData("on")]
    public void GivenAValueThatIsNotTrueOrFalse_WhenParsingBool_ThenNullIsReturned(string value)
    {
        var key = NewKey();

        Assert.Null(ProviderWith(key, value).GetBool(key));
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("-5", -5)]
    [InlineData("+7", 7)]
    [InlineData(" 42 ", 42)]
    [InlineData("2147483647", int.MaxValue)]
    [InlineData("-2147483648", int.MinValue)]
    public void GivenAnIntegerUnderPtBr_WhenParsingInt_ThenItIsReadInvariantly(string value, int expected)
    {
        var key = NewKey();
        using var culture = new CultureScope("pt-BR");

        Assert.Equal(expected, ProviderWith(key, value).GetInt(key));
    }

    [Theory]
    [InlineData("2147483648")]
    [InlineData("-2147483649")]
    [InlineData("1.000")]
    [InlineData("1,5")]
    [InlineData("0x1F")]
    [InlineData("12x")]
    public void GivenAValueThatIsNotAnInt32UnderPtBr_WhenParsingInt_ThenNullIsReturned(string value)
    {
        // pt-BR groups thousands with a dot and has a decimal comma: neither is read as a number.
        var key = NewKey();
        using var culture = new CultureScope("pt-BR");

        Assert.Null(ProviderWith(key, value).GetInt(key));
    }

    [Fact]
    public void GivenAnEmptyValue_WhenParsing_ThenNoTypedValueIsReturned()
    {
        // Whether an empty environment variable exists at all depends on the platform; either way nothing parses.
        var key = NewKey();
        var provider = ProviderWith(key, string.Empty);

        Assert.Null(provider.GetBool(key));
        Assert.Null(provider.GetInt(key));
        Assert.Null(provider.GetObject<Person>(key));
    }

    [Fact]
    public void GivenAValueWithSurroundingWhitespace_WhenGettingString_ThenItIsReturnedUntouched()
    {
        var key = NewKey();

        Assert.Equal("  padded  ", ProviderWith(key, "  padded  ").GetString(key));
    }

    [Fact]
    public void GivenJsonWithPropertiesInAnotherCasing_WhenParsingObject_ThenThePropertiesAreMatched()
    {
        var key = NewKey();

        var person = ProviderWith(key, """{"name":"Ana","AGE":25,"unknown":true}""").GetObject<Person>(key);

        Assert.NotNull(person);
        Assert.Equal("Ana", person.Name);
        Assert.Equal(25, person.Age);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("\"Ana\"")]
    [InlineData("42")]
    [InlineData("""{"Age":"not a number"}""")]
    public void GivenJsonThatIsNotTheObject_WhenParsingObject_ThenNullIsReturned(string value)
    {
        var key = NewKey();

        Assert.Null(ProviderWith(key, value).GetObject<Person>(key));
    }
}
