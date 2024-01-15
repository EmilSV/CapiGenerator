using CapiGenerator.Parser;
using CppAst;

string headerFile = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

if (!File.Exists(headerFile))
{
    Console.WriteLine($"Header file {headerFile} does not exist.");
    return 1;
}

var options = new CppParserOptions
{
    ParseMacros = true,
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

var compilationUnit = new CCompilationUnit();

compilationUnit.AddParser([
    new ConstantParser(),
    new EnumParser(),
    new FunctionParser(),
    new StructParser(),
    new TypedefParser()
]);


compilationUnit.Parse([cppCompilation]);

foreach (var constant in compilationUnit.GetConstantEnumerable())
{
    Console.WriteLine(constant.Name);
}


return 0;