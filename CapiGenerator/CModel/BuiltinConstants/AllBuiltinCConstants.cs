using System.Collections.Immutable;

namespace CapiGenerator.CModel.BuiltinConstants;

public class AllBuiltinCConstants
{
    public static readonly ImmutableArray<BaseBuiltInCConstant> AllCConstantTypes = [
        SizeMaxBuiltinConstant.Instance,
        Int8MaxBuiltinConstant.Instance,
        Int8MinBuiltinConstant.Instance,
        Int16MaxBuiltinConstant.Instance,
        Int16MinBuiltinConstant.Instance,
        Int32MaxBuiltinConstant.Instance,
        Int32MinBuiltinConstant.Instance,
        Int64MaxBuiltinConstant.Instance,
        Int64MinBuiltinConstant.Instance,
        IntPtrMaxBuiltinConstant.Instance,
        IntPtrMinBuiltinConstant.Instance,
        SizeMaxBuiltinConstant.Instance,
        UInt8MaxBuiltinConstant.Instance,
        UInt16MaxBuiltinConstant.Instance,
        UInt32MaxBuiltinConstant.Instance,
        UInt64MaxBuiltinConstant.Instance,
        UIntPtrMaxBuiltinConstant.Instance,
        NanBuiltinConstant.Instance,
    ];
}