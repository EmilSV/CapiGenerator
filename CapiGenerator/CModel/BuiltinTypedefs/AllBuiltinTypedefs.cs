using System.Collections.Immutable;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class AllBuiltinTypedefs
{
    public static readonly ImmutableArray<BaseBuiltinTypedef> AllTypedefs = [
            Int8TBuiltinTypedef.Instance,
            Int16TBuiltinTypedef.Instance,
            Int32TBuiltinTypedef.Instance,
            Int64TBuiltinTypedef.Instance,
            IntFast8TBuiltinTypedef.Instance,
            IntFast16TBuiltinTypedef.Instance,
            IntFast32TBuiltinTypedef.Instance,
            IntFast64TBuiltinTypedef.Instance,
            IntLeast8TBuiltinTypedef.Instance,
            IntLeast16TBuiltinTypedef.Instance,
            IntLeast32TBuiltinTypedef.Instance,
            IntLeast64TBuiltinTypedef.Instance,
            IntMaxTBuiltinTypedef.Instance,
            UInt8TBuiltinTypedef.Instance,
            UInt16TBuiltinTypedef.Instance,
            UInt32TBuiltinTypedef.Instance,
            UInt64TBuiltinTypedef.Instance,
            UIntFast8TBuiltinTypedef.Instance,
            UIntFast16TBuiltinTypedef.Instance,
            UIntFast32TBuiltinTypedef.Instance,
            UIntFast64TBuiltinTypedef.Instance,
            UIntLeast8TBuiltinTypedef.Instance,
            UIntLeast16TBuiltinTypedef.Instance,
            UIntLeast32TBuiltinTypedef.Instance,
            UIntLeast64TBuiltinTypedef.Instance,
            UIntMaxTBuiltinTypedef.Instance,
    ];
}