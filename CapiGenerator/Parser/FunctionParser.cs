using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.Parser;

public class FunctionParser : BaseParser
{
    public override void FirstPass(ReadOnlySpan<CppCompilation> compilations, BaseParserOutputChannel outputChannel)
    {

        foreach (var compilation in compilations)
        {
            foreach (var cppFunction in compilation.Functions)
            {
                if (cppFunction == null)
                {
                    continue;
                }

                if (ShouldSkip(cppFunction))
                {
                    continue;
                }

                var newFunction = FirstPass(cppFunction);
                if (newFunction is not null)
                {
                    outputChannel.OnReceiveFunction(newFunction);
                }
            }
        }
    }

    public override void SecondPass(
        CCompilationUnit compilationUnit,
        BaseParserInputChannel inputChannel)
    {
        foreach (var function in inputChannel.GetFunctions())
        {
            function.OnSecondPass(compilationUnit);
        }
    }

    protected virtual CFunction? FirstPass(CppFunction function)
    {
        switch (CFunction.From(function))
        {
            case { } cFunction:
                return cFunction;
            default:
                OnError(function, "Failed to parse parameters");
                return null;
        }

    }

    protected virtual bool ShouldSkip(CppFunction function)
    {
        return FakeCStdHeader.IsFakeStdHeaderFile(function.Span.Start.File) ||
            function.Parameters.Any(i => i.Type.TypeKind == CppTypeKind.Typedef && i.Type.GetDisplayName() == "va_list");
    }
    protected virtual void OnError(CppFunction constant, string message)
    {
        Console.Error.WriteLine($"Error parsing function {constant.Name}: {message}");
    }
}
