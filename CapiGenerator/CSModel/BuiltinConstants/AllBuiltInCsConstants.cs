using System.Collections.Immutable;

namespace CapiGenerator.CSModel.BuiltinConstants;

public static class AllBuiltInCsConstants
{
    public static readonly ImmutableArray<BaseBuiltInCsConstant> csConstants = [
        NuintMaxValue.Instance
    ];
}