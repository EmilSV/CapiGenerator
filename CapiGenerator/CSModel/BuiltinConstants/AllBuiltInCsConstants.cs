using System.Collections.Immutable;

namespace CapiGenerator.CSModel.BuiltinConstants;

public static class AllBuiltInCsConstants
{
    public static readonly ImmutableArray<BaseBuiltInCsConstant> csConstants = [
        NuintMaxValue.Instance,
        ByteMaxValue.Instance,
        IntMax.Instance,
        IntMinValue.Instance,
        LongMaxValue.Instance,
        LongMinValue.Instance,
        NintMaxValue.Instance,
        NintMinValue.Instance,
        NuintMaxValue.Instance,
        SByteMaxValue.Instance,
        SByteMinValue.Instance,
        ShortMaxValue.Instance,
        ShortMinValue.Instance,
        UIntMaxValue.Instance,
        ULongMaxValue.Instance,
        UShortMaxValue.Instance
    ];
}