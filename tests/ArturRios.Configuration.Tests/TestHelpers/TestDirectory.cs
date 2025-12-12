namespace ArturRios.Configuration.Tests.TestHelpers;

/// <summary>
///     Creates and manages a temporary directory for filesystem-based tests.
/// </summary>
public sealed class TestDirectory : IDisposable
{
    public TestDirectory()
    {
        Root = Path.Combine(Path.GetTempPath(), "ArturRios.Configuration.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }
        catch
        {
            // Best effort cleanup; ignore exceptions from locked files.
        }
    }

    public void EnsureSubfolder(string relativeFolder)
    {
        var path = Path.Combine(Root, relativeFolder);
        Directory.CreateDirectory(path);
    }

    public void WriteFile(string relativePath, string content)
    {
        var fullPath = Path.Combine(Root, relativePath);
        var dir = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(dir);

        File.WriteAllText(fullPath, content);
    }
}
