using CapiGenerator.CModel;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel.Type;

public class CTypeInstance : BaseCAstItem
{
    private readonly CTypeModifier[]? _modifiers;
    public ResoleRef<ICType, string> CTypeRef { get; }
    public ReadOnlySpan<CTypeModifier> Modifiers =>
        _modifiers ?? ReadOnlySpan<CTypeModifier>.Empty;

    public CTypeInstance(Guid compilationUnitId, ICType cType, ReadOnlySpan<CTypeModifier> modifiers)
        : base(compilationUnitId)
    {
        CTypeRef = new(cType);
        _modifiers = modifiers.ToArray();
    }

    public CTypeInstance(Guid compilationUnitId, string typeName, ReadOnlySpan<CTypeModifier> modifiers)
        : base(compilationUnitId)
    {
        CTypeRef = new(typeName);
        _modifiers = modifiers.ToArray();
    }

    public bool GetIsCompletedType()
    {
        return CTypeRef.IsOutputResolved();
    }

    public ICType? GetCType()
    {
        return CTypeRef.Output;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (compilationUnit.CompilationUnitId != CompilationUnitId)
        {
            throw new InvalidOperationException("Compilation unit id mismatch");
        }

        CTypeRef.TrySetOutputFromResolver(compilationUnit);
    }
}
