using System.Globalization;

namespace ArturRios.Configuration.Tests.TestHelpers;

/// <summary>
///     Runs a test under a given culture and puts the previous one back, so a culture-sensitive assertion does not
///     depend on the machine it runs on (the owner's is pt-BR, CI's is invariant).
/// </summary>
public sealed class CultureScope : IDisposable
{
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;

    public CultureScope(string name)
    {
        CultureInfo.CurrentCulture = new CultureInfo(name);
        CultureInfo.CurrentUICulture = new CultureInfo(name);
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _uiCulture;
    }
}
