
using CapiGenerator;
using CppAst;

if (args.Length < 2)
{
    Console.WriteLine("Usage: CapiGenerator.exe <header file> <output folder>");
    return 1;
}

string headerFile = Path.GetFullPath(args[0]);
string outputFolder = Path.GetFullPath(args[1]);

if (!File.Exists(headerFile))
{
    Console.WriteLine($"Header file {headerFile} does not exist.");
    return 1;
}

if (!Directory.Exists(outputFolder))
{
    Console.WriteLine($"Output folder {outputFolder} does not exist.");
    return 1;
}

var options = new CppParserOptions
{
    ParseMacros = true,
    Defines = { },
};

var cppCompilation = CppParser.ParseFile(headerFile, options);

if (cppCompilation.HasErrors)
{
    Console.WriteLine("Errors occurred while parsing the header file.");

    foreach (var error in cppCompilation.Diagnostics.Messages)
    {
        Console.WriteLine(error);
    }

    return 1;
}

var generator = new CapiGeneratorUnit();

generator.AddCompilation(cppCompilation, Path.GetFileNameWithoutExtension(headerFile), outputFolder);
await generator.WriteToDiskAsync();

return 0;
