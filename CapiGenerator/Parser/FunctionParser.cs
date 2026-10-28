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
            IsNotExportedInline(function) ||
            IsReservedIdentifier(function.Name) ||
            function.Flags.HasFlag(CppFunctionFlags.Variadic) || // varargs (...) are not supported yet

            function.Parameters.Any(i => i.Type.TypeKind == CppTypeKind.Typedef && i.Type.GetDisplayName() == "va_list");
    }

    // Header inline functions (e.g. SDL_FORCE_INLINE) have no symbol in the library, so binding them
    // throws EntryPointNotFoundException. Only `extern inline` emits an exported symbol.
    // StorageQualifier alone is not enough: MSVC's __forceinline is not `static`.
    protected static bool IsNotExportedInline(CppFunction function) =>
        function.Flags.HasFlag(CppFunctionFlags.Inline) &&
        function.StorageQualifier != CppStorageQualifier.Extern;

    // C reserves `__x` and `_X` names for the compiler and standard library (e.g. MSVC's __debugbreak),
    // so they are not part of the library's API and usually have no symbol in it.
    protected static bool IsReservedIdentifier(string name) =>
        name.StartsWith("__", StringComparison.Ordinal) ||
        name.Length > 1 && name[0] == '_' && char.IsAsciiLetterUpper(name[1]);

    protected virtual void OnError(CppFunction constant, string message)
    {
        Console.Error.WriteLine($"Error parsing function {constant.Name}: {message}");
    }
}
