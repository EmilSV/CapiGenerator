using System.Diagnostics.CodeAnalysis;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.CFFITypes;

public sealed class CWCharTType : BaseBuiltinType
{
    public static CWCharTType Instance { get; } = new();

    public override string Namespace => "CFFITypes";

    public override string Name { get; } = "CWCharT";
}
