namespace CapiGenerator;

public static class FakeCStdHeader
{
    public const string POSTFIX = "_V0_0_1";

    public static string CreateFakeStdHeaderFolder(string? path = null)
    {
        var assembly = typeof(FakeCStdHeader).Assembly;
        var resourceNames = assembly.GetManifestResourceNames().Where(i => i.StartsWith("CapiGenerator.FakeCStdHeaders.")).ToList();
        var outputFolder = path ?? Path.Combine(Path.GetTempPath(), $"FakeCStdHeaders{POSTFIX}");
        Directory.CreateDirectory(outputFolder);
        foreach (var resourceName in resourceNames)
        {
            var fileName = resourceName["CapiGenerator.FakeCStdHeaders.".Length..];
            var outputPath = Path.Combine(outputFolder, fileName);
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var fileStream = File.Create(outputPath);
            stream.CopyTo(fileStream);
        }

        return outputFolder;
    }
}