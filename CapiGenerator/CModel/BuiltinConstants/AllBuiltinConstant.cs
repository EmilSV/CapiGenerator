using System.Collections.Immutable;

namespace CapiGenerator.CModel.BuiltinConstants;

public class AllBuiltinConstant
{
    public static readonly ImmutableArray<BaseBuiltInCConstant> AllCConstantTypes = [
        SizeMaxBuiltinConstant.Instance
    ];
}