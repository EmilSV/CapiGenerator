using CapiGenerator.CModel;

using CppAst;
using System.Diagnostics;

namespace CapiGenerator.Parser;

public class UnionParser : BaseParser
{
    private NestedCTypeFactory _nestedCTypeFactory => field ??= new NestedCTypeFactory(Parsers);

    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var cppUnion in compilation.Classes)
            {
                if (cppUnion == null)
                {
                    continue;
                }

                if (ShouldSkip(cppUnion))
                {
                    continue;
                }

                var newUnion = FirstPass(cppUnion);
                if (newUnion is not null)
                {
                    outputChannel.OnReceiveUnion(newUnion);
                }
            }
        }
    }

    protected virtual CUnion? FirstPass(CppClass cppUnion)
    {
        var cUnion = CppClassToCUnion(cppUnion);
        if (cUnion is null)
        {
            OnError(cppUnion, "Failed to parse fields");
            return null;
        }

        return cUnion;
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var cUnion in inputChannel.GetUnions())
        {
            cUnion.OnSecondPass(compilationUnit);
        }
    }

    protected virtual bool ShouldSkip(CppClass cppUnion) =>
        cppUnion.IsAnonymous || cppUnion.Parent is CppClass || cppUnion.ClassKind != CppClassKind.Union;

    protected virtual void OnError(CppClass cppUnion, string message)
    {
        Console.Error.WriteLine($"Error parsing union {cppUnion.Name}: {message}");
    }

    public virtual CUnion? CppClassToCUnion(
        CppClass cppUnion,
        string? nameOverride = null,
        bool isAnonymous = false,
        object? secondarySource = null)
    {
        List<ICType> nestedTypes = [];
        var fields = cppUnion.Fields
            .Select(i => CField.FromCppField(cppUnion, i, _nestedCTypeFactory, nestedTypes))
            .ToArray();
        if (fields == null || fields.Any(field => field is null))
        {
            return null;
        }

        var cUnion = new CUnion(
            cppUnion,
            nameOverride ?? cppUnion.Name,
            fields!,
            isAnonymous,
            nestedTypes.ToArray());
        if (secondarySource is not null)
        {
            cUnion.AddSecondarySource(secondarySource);
        }

        return cUnion;
    }
}
