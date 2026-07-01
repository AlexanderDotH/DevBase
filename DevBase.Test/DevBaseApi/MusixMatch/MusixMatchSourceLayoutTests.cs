namespace DevBase.Test.DevBaseApi.MusixMatch;

public sealed class MusixMatchSourceLayoutTests
{
    private const int MaxDirectSourceFilesPerFolder = 10;

    [TestCase("Structure/Requests")]
    [TestCase("Structure/Types")]
    [TestCase("Json")]
    public void CrowdedMusixMatchFolders_DoNotKeepSourceFilesDirectly(string relativePath)
    {
        string directory = MusixMatchDirectory(relativePath);
        if (!Directory.Exists(directory)) {
            return;
        }

        string[] directFiles = Directory
            .EnumerateFiles(directory, "*.cs", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray()!;

        Assert.That(
            directFiles,
            Is.Empty,
            $"{relativePath} should be split into domain folders instead of keeping DTO files directly.");
    }

    [Test]
    public void MusixMatchSourceFolders_AreSplitBeforeTheyBecomeTooLarge()
    {
        string root = MusixMatchDirectory();
        string[] crowdedFolders = Directory
            .EnumerateDirectories(root, "*", SearchOption.AllDirectories)
            .Prepend(root)
            .Select(path => new
            {
                Path = Path.GetRelativePath(root, path),
                Count = Directory.EnumerateFiles(path, "*.cs", SearchOption.TopDirectoryOnly).Count()
            })
            .Where(folder => folder.Count > MaxDirectSourceFilesPerFolder)
            .Select(folder => $"{folder.Path}: {folder.Count}")
            .OrderBy(entry => entry, StringComparer.Ordinal)
            .ToArray();

        Assert.That(
            crowdedFolders,
            Is.Empty,
            $"Musixmatch source folders should have at most {MaxDirectSourceFilesPerFolder} direct .cs files before being split by domain.");
    }

    private static string MusixMatchDirectory(string relativePath = "")
    {
        string root = Path.Combine(
            MusixMatchTestSupport.RepositoryRoot,
            "DevBase.Api",
            "Apis",
            "Musixmatch");

        if (string.IsNullOrWhiteSpace(relativePath)) {
            return root;
        }

        string[] parts = relativePath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
        return Path.Combine([root, .. parts]);
    }
}
