/*
    Base on  https://github.com/EvergineTeam/WebGPU.NET/tree/master license: MIT 
    https://github.com/EvergineTeam/WebGPU.NET/blob/master/LICENSE.txt
*/

using System.IO;
using CapiGenerator.Parsers;
using CppAst;

internal class Program
{
    private static int Main(string[] args)
    {
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
            Defines = {
            },
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


        new ConstantParser().Create(cppCompilation);


        return 0;
    }
}