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
        var parameters = function.Parameters.Select(CppParameterToCParameter).ToArray();
        if (parameters == null || parameters.Any(parameter => parameter is null))
        {
            OnError(function, "Failed to parse parameters");
            return null;
        }

        var returnType = CTypeInstance.FromCppType(function.ReturnType);

        return new CFunction(returnType, function.Name, parameters!);

    }

    protected virtual bool ShouldSkip(CppFunction constant) => false;
    protected virtual void OnError(CppFunction constant, string message)
    {
        Console.Error.WriteLine($"Error parsing function {constant.Name}: {message}");
    }

    private static CParameter CppParameterToCParameter(CppParameter parameter)
    {
        var parameterType = CTypeInstance.FromCppType(parameter.Type);
        return new CParameter(parameter.Name, parameterType);
    }
}