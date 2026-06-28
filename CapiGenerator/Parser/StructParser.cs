using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.Parser;


public class StructParser : BaseParser
{
    private AnonymousCTypeFactory _anonymousCTypeFactory => field ??= new AnonymousCTypeFactory(Parsers);

    public override void Init(ParserCollection parsers)
    {
        base.Init(parsers);
    }

    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var cppStruct in compilation.Classes)
            {
                if (cppStruct == null)
                {
                    continue;
                }

                if (ShouldSkip(cppStruct))
                {
                    continue;
                }

                var newStruct = FirstPass(cppStruct);
                if (newStruct is not null)
                {
                    outputChannel.OnReceiveStruct(newStruct);
                }
            }
        }
    }

    protected virtual CStruct? FirstPass(CppClass cppStruct)
    {
        var cStruct = CppClassToCStruct(cppStruct);
        if (cStruct is null)
        {
            OnError(cppStruct, "Failed to parse fields");
            return null;
        }
        return cStruct;
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var cStruct in inputChannel.GetStructs())
        {
            cStruct.OnSecondPass(compilationUnit);
        }
    }

    protected virtual bool ShouldSkip(CppClass cppStruct) =>
        cppStruct.IsAnonymous || cppStruct.ClassKind == CppClassKind.Union;
    protected virtual void OnError(CppClass cppStruct, string message)
    {
        Console.Error.WriteLine($"Error parsing struct {cppStruct.Name}: {message}");
    }

    public virtual CStruct? CppClassToCStruct(
        CppClass cppStruct,
        string? nameOverride = null,
        bool isAnonymous = false)
    {
        var fields = cppStruct.Fields.Select(i => CField.FromCppField(i, _anonymousCTypeFactory)).ToArray();
        if (fields == null || fields.Any(field => field is null))
        {
            return null;
        }

        return new CStruct(nameOverride ?? cppStruct.Name, fields!, isAnonymous);
    }
}
