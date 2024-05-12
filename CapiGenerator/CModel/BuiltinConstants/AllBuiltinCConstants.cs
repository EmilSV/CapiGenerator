using System.Collections.Immutable;

namespace CapiGenerator.CModel.BuiltinConstants;

public class AllBuiltinCConstants
{
    public static readonly ImmutableArray<BaseBuiltInCConstant> AllCConstantTypes = [
        SizeMaxBuiltinConstant.Instance
    ];
}